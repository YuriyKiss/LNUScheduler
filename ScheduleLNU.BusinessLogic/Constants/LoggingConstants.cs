namespace ScheduleLNU.BusinessLogic.Constants
{
    public static class LoggingConstants
    {
        public const string StudentId = "Student with id {studentId}";

        public const string StudentNotFound = StudentId + " is not found.";

        public const string StudentHasNoSelectedTheme = StudentId + " has no selected theme.";

        public const string StudentViewedThemes = StudentId + " viewed {amound} themes.";

        public const string StudentHasNoThemes = StudentId + " has no themes.";

        public const string StudentHasNoTheme = StudentId + " has no theme with id {themeId}.";

        public const string StudentViewedTheme = StudentId + " viewed theme with id {themeId}.";

        public const string StudentChangedTheme = StudentId + " changed theme with id {theme id}.";

        public const string SelectedThemeWasChanded = StudentChangedTheme + " and cookies was updated.";

        public const string StudentSelectsTheme = StudentId + " selects theme with id {themeid}.";

        public const string StudentDeselectsTheme = StudentId + " deselects theme with id {themeid}.";

        public const string StudentHasInvalidTheme = StudentId + " work with the invalid theme {themeid}.";

        public const string StudentDeletesTheme = StudentId + " deletes theme with id {themeId}";
    }
}
