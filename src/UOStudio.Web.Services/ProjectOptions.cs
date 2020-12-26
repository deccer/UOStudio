namespace UOStudio.Web.Services
{
    public class ProjectOptions
    {
        public const string ProjectSettings = nameof(ProjectSettings);

        public string TemplatePath { get; set; }

        public string ProjectsPath { get; set; }

        public string TempPath { get; set; }
    }
}
