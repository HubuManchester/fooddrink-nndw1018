using System.Diagnostics;

namespace FoodieApp.Helpers;

/// <summary>
/// Cross-platform TTS helper. Uses native WinRT SpeechSynthesizer/MediaPlayer
/// on Windows because MAUI's TextToSpeech abstraction is unreliable on some
/// Windows builds. Falls back to MAUI TextToSpeech on other platforms.
///
/// Supports pause / resume via the static PauseAsync / ResumeAsync methods,
/// and full stop via StopAsync. Callers should use StopAsync when leaving a
/// page so audio does not keep playing in the background.
/// </summary>
public static class TextToSpeechHelper
{
    private static CancellationTokenSource? _globalCts;
    private static bool _isPaused;

#if WINDOWS
    private static Windows.Media.Playback.MediaPlayer? _currentPlayer;
#endif

    /// <summary>
    /// Whether TTS is currently paused (audio playback paused mid-stream).
    /// </summary>
    public static bool IsPaused => _isPaused;

    /// <summary>
    /// Speaks the given text. If another utterance is already playing it will
    /// be stopped first. Pass a CancellationToken to cooperatively cancel.
    /// On Windows the token's Cancel() will pause the player rather than
    /// dispose it, so that ResumeAsync can continue playback.
    /// </summary>
    public static async Task SpeakAsync(string text, CancellationToken cancelToken = default)
    {
        // Stop any previous utterance
        await StopAsync();

#if WINDOWS
        await SpeakWindowsAsync(text, cancelToken);
#else
        await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions
        {
            Pitch = 1.0f,
            Volume = 1.0f
        }, cancelToken);
#endif
    }

    /// <summary>
    /// Pauses the current utterance without disposing resources so playback
    /// can be resumed later.
    /// </summary>
    public static Task PauseAsync()
    {
        _isPaused = true;
#if WINDOWS
        try
        {
            _currentPlayer?.Pause();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"TTS pause error: {ex.Message}");
        }
#endif
        return Task.CompletedTask;
    }

    /// <summary>
    /// Resumes a previously paused utterance. If no utterance is paused this
    /// is a no-op.
    /// </summary>
    public static Task ResumeAsync()
    {
        _isPaused = false;
#if WINDOWS
        try
        {
            _currentPlayer?.Play();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"TTS resume error: {ex.Message}");
        }
#endif
        return Task.CompletedTask;
    }

    /// <summary>
    /// Fully stops and cleans up the current utterance. Call this when leaving
    /// a page so audio does not keep playing in the background.
    /// </summary>
    public static Task StopAsync()
    {
        _isPaused = false;
        try
        {
            _globalCts?.Cancel();
            _globalCts?.Dispose();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"TTS stop CTS error: {ex.Message}");
        }
        _globalCts = null;

#if WINDOWS
        try
        {
            _currentPlayer?.Pause();
            _currentPlayer?.Dispose();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"TTS stop player error: {ex.Message}");
        }
        _currentPlayer = null;
#endif
        return Task.CompletedTask;
    }

    public static async Task<IEnumerable<Locale>> GetLocalesAsync()
    {
        try
        {
            return await TextToSpeech.Default.GetLocalesAsync();
        }
        catch
        {
            return Enumerable.Empty<Locale>();
        }
    }

#if WINDOWS
    private static async Task SpeakWindowsAsync(string text, CancellationToken cancelToken)
    {
        try
        {
            var synthesizer = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();

            // Try to find an English voice, fall back to default
            var voices = Windows.Media.SpeechSynthesis.SpeechSynthesizer.AllVoices;
            var englishVoice = voices.FirstOrDefault(v =>
                v.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));
            if (englishVoice != null)
            {
                synthesizer.Voice = englishVoice;
            }

            var stream = await synthesizer.SynthesizeTextToStreamAsync(text);

            var player = new Windows.Media.Playback.MediaPlayer();
            _currentPlayer = player;  // store for pause/resume/stop

            var source = Windows.Media.Core.MediaSource.CreateFromStream(stream, stream.ContentType);
            player.Source = source;

            var tcs = new TaskCompletionSource<bool>();
            player.MediaEnded += (s, e) =>
            {
                _isPaused = false;
                tcs.TrySetResult(true);
            };
            player.MediaFailed += (s, e) =>
            {
                Debug.WriteLine($"TTS playback failed: {e.ErrorMessage}");
                _isPaused = false;
                tcs.TrySetResult(false);
            };

            // Handle cancellation — pause instead of full stop so the caller
            // can choose to resume or fully stop later.
            if (cancelToken != default)
            {
                cancelToken.Register(() =>
                {
                    // Pause and signal completion
                    player.Pause();
                    _isPaused = true;
                    tcs.TrySetResult(false);
                });
            }

            // Link the global CTS so StopAsync cancels this as well
            _globalCts?.Dispose();
            _globalCts = new CancellationTokenSource();
            _globalCts.Token.Register(() =>
            {
                try { player.Pause(); player.Dispose(); }
                catch (Exception ex) { Debug.WriteLine($"TTS global stop cleanup: {ex.Message}"); }
                _isPaused = false;
            });

            player.Play();
            await tcs.Task;

            // If not paused (i.e., completed naturally or fully stopped),
            // clean up the player.
            if (!_isPaused)
            {
                player.Dispose();
                _currentPlayer = null;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Windows TTS error: {ex.Message}");
            throw;
        }
    }
#endif
}
