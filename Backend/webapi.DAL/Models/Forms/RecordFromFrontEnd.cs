public class RecordFromFrontEnd
{
    public int selectedYear { get; set; }
    public int selectedMonth { get; set; }
    public int selectedDay { get; set; }
    public bool showGroupList { get; set; }
    public bool yourSelf { get; set; }
    public string Creator { get; set; }
    public string selectedObject { get; set; }
    public int importance { get; set; }
    public int hour { get; set; }
    public int minute { get; set; }
    public string recordName { get; set; }
    public string recordContent { get; set; }

    public bool isValid()
    {
        if (!isDateValid() ||
            string.IsNullOrEmpty(selectedObject) ||
            !(0 <= hour && hour <= 23) ||
            !(0 <= minute && minute <= 59) ||
            !(recordName.Length >= 1 && recordName.Length <= 50) ||
            !(recordContent.Length >= 1 && recordContent.Length <= 500) ||
            !(importance >= 0 && importance <= 2)
            )
            return false;
        return true;
    }

    private bool isDateValid()
    {
        string format = "yyyy-MM-dd HH:mm";
        string inputDate = $"{selectedYear:D4}-{selectedMonth:D2}-{selectedDay:D2} {hour:D2}:{minute:D2}";

        // Try parsing the input string into a DateTime object
        if (DateTime.TryParseExact(inputDate, format, null, System.Globalization.DateTimeStyles.None, out _))
            return true;
        else
            return false;
    }
}