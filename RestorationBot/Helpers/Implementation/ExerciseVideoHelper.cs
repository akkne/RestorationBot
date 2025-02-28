namespace RestorationBot.Helpers.Implementation;

using Abstract;
using global::Telegram.Bot.Types;
using Models.Request;

public class ExerciseVideoHelper : IExerciseVideoHelper
{
    private readonly ILogger<ExerciseVideoHelper> _logger;

    public ExerciseVideoHelper(ILogger<ExerciseVideoHelper> logger)
    {
        _logger = logger;
    }

    public List<InputFile> GetExerciseVideos(ExerciseMessageInformation messageInformation, int exerciseIndex)
    {
        List<InputFile> result = [];
        string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Exercise");
        
        string multipleVideosPattern =
            $"{(int)messageInformation.RestorationStep + 1}.{messageInformation.ExerciseType + 1}.{exerciseIndex + 1}.*.mp4";
        _logger.LogInformation($"Getting exercise messages with pattern: {multipleVideosPattern}");

        List<string> multipleVideos = Directory.GetFiles(baseDirectory, multipleVideosPattern)
                                               .OrderBy(f => f)
                                               .ToList();

        _logger.LogInformation($"Found {multipleVideos.Count} multiple videos");

        if (multipleVideos.Count != 0)
        {
            result.AddRange(multipleVideos.Select(video =>
                InputFile.FromStream(new FileStream(video, FileMode.Open, FileAccess.Read))));
            return result;
        }
        
        string singleVideoPattern =
            $"{(int)messageInformation.RestorationStep + 1}.{messageInformation.ExerciseType + 1}.{exerciseIndex + 1}.mp4";
        _logger.LogInformation($"Getting exercise messages with pattern: {singleVideoPattern}");
        string singleVideoPath = Path.Combine(baseDirectory, singleVideoPattern);
        _logger.LogInformation($"Trying to get file: {singleVideoPath}");

        if (File.Exists(singleVideoPath))
        {
            _logger.LogInformation($"Single video found at: {singleVideoPath}");
            result.Add(InputFile.FromStream(new FileStream(singleVideoPath, FileMode.Open, FileAccess.Read)));
        }

        return result;
    }
}