﻿using Microsoft.ML;

namespace GitHubIssueClassification;

internal class Program
{
    private static MLContext _mlContext;
    private static PredictionEngine<GitHubIssue, IssuePrediction> _predEngine;
    private static ITransformer _trainedModel;
    private static IDataView _trainingDataView;

    private static string AppPath => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
    private static string TrainDataPath => Path.Combine(AppPath, "..", "..", "..", "Data", "issues_train.tsv");
    private static string TestDataPath => Path.Combine(AppPath, "..", "..", "..", "Data", "issues_test.tsv");

    private static string ModelPath => Path.Combine(AppPath, "..", "..", "..", "Models", "model.zip");

    private static void Main(string[] args)
    {
        // Create MLContext to be shared across the model creation workflow objects
        // Set a random seed for repeatable/deterministic results across multiple trainings.
        _mlContext = new MLContext(0);

        // // STEP 1: Common data loading configuration
        // // CreateTextReader<GitHubIssue>(hasHeader: true) - Creates a TextLoader by inferencing the dataset schema from the GitHubIssue data model type.
        // // .Read(_trainDataPath) - Loads the training text file into an IDataView (_trainingDataView) and maps from input columns to IDataView columns.
        // Console.WriteLine("=============== Loading Dataset  ===============");
        // _trainingDataView = _mlContext.Data.LoadFromTextFile<GitHubIssue>(TrainDataPath, hasHeader: true);
        // Console.WriteLine("=============== Finished Loading Dataset  ===============");
        //
        // var pipeline = ProcessData();
        // var trainingPipeline = BuildAndTrainModel(_trainingDataView, pipeline);
        // Evaluate(_trainingDataView.Schema);
        PredictIssue();
    }

    public static IEstimator<ITransformer> ProcessData()
    {
        Console.WriteLine("=============== Processing Data ===============");
        // STEP 2: Common data process configuration with pipeline data transformations
        var pipeline = _mlContext.Transforms.Conversion
            .MapValueToKey(inputColumnName: "Area", outputColumnName: "Label")
            .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Title",
                outputColumnName: "TitleFeaturized"))
            .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Description",
                outputColumnName: "DescriptionFeaturized"))
            .Append(_mlContext.Transforms.Concatenate("Features", "TitleFeaturized", "DescriptionFeaturized"))
            //Sample Caching the DataView so estimators iterating over the data multiple times, instead of always reading from file, using the cache might get better performance.
            .AppendCacheCheckpoint(_mlContext);
        
        Console.WriteLine("=============== Finished Processing Data ===============");
        
        return pipeline;
    }

    public static IEstimator<ITransformer> BuildAndTrainModel(IDataView trainingDataView,
        IEstimator<ITransformer> pipeline)
    {
        // STEP 3: Create the training algorithm/trainer
        // Use the multi-class SDCA algorithm to predict the label using features.
        //Set the trainer/algorithm and map label to value (original readable state)
        var trainingPipeline = pipeline.Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
        
        // STEP 4: Train the model fitting to the DataSet
        Console.WriteLine("=============== Training the model  ===============");

        _trainedModel = trainingPipeline.Fit(trainingDataView);
        Console.WriteLine(
            $"=============== Finished Training the model Ending time: {DateTime.Now.ToString()} ===============");

        // (OPTIONAL) Try/test a single prediction with the "just-trained model" (Before saving the model)
        Console.WriteLine("=============== Single Prediction just-trained-model ===============");

        // Create prediction engine related to the loaded trained model
        _predEngine = _mlContext.Model.CreatePredictionEngine<GitHubIssue, IssuePrediction>(_trainedModel);
        var issue = new GitHubIssue
        {
            Title = "WebSockets communication is slow in my machine",
            Description =
                "The WebSockets communication used under the covers by SignalR looks like is going slow in my development machine.."
        };

        var prediction = _predEngine.Predict(issue);
        Console.WriteLine(
            $"=============== Single Prediction just-trained-model - Result: {prediction.Area} ===============");

        return trainingPipeline;
    }

    public static void Evaluate(DataViewSchema trainingDataViewSchema)
    {
        // STEP 5:  Evaluate the model in order to get the model's accuracy metrics
        Console.WriteLine(
            $"=============== Evaluating to get model's accuracy metrics - Starting time: {DateTime.Now.ToString()} ===============");

        //Load the test dataset into the IDataView
        var testDataView = _mlContext.Data.LoadFromTextFile<GitHubIssue>(TestDataPath, hasHeader: true);

        //Evaluate the model on a test dataset and calculate metrics of the model on the test data.
        var testMetrics = _mlContext.MulticlassClassification.Evaluate(_trainedModel.Transform(testDataView));

        Console.WriteLine(
            $"=============== Evaluating to get model's accuracy metrics - Ending time: {DateTime.Now.ToString()} ===============");
        Console.WriteLine(
            "*************************************************************************************************************");
        Console.WriteLine("*       Metrics for Multi-class Classification model - Test Data     ");
        Console.WriteLine(
            "*------------------------------------------------------------------------------------------------------------");
        Console.WriteLine($"*       MicroAccuracy:    {testMetrics.MicroAccuracy:0.###}");
        Console.WriteLine($"*       MacroAccuracy:    {testMetrics.MacroAccuracy:0.###}");
        Console.WriteLine($"*       LogLoss:          {testMetrics.LogLoss:#.###}");
        Console.WriteLine($"*       LogLossReduction: {testMetrics.LogLossReduction:#.###}");
        Console.WriteLine(
            "*************************************************************************************************************");
        SaveModelAsFile(_mlContext, trainingDataViewSchema, _trainedModel);
    }

    public static void PredictIssue()
    {
        var loadedModel = _mlContext.Model.Load(ModelPath, out var modelInputSchema);
        var singleIssue = new GitHubIssue
            { Title = "Entity Framework crashes", Description = "When connecting to the database, EF is crashing" };
        _predEngine = _mlContext.Model.CreatePredictionEngine<GitHubIssue, IssuePrediction>(loadedModel);

        var prediction = _predEngine.Predict(singleIssue);

        Console.WriteLine($"=============== Single Prediction - Result: {prediction.Area} ===============");
    }

    private static void SaveModelAsFile(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
    {
        mlContext.Model.Save(model, trainingDataViewSchema, ModelPath);
        Console.WriteLine("The model is saved to {0}", ModelPath);
    }
}