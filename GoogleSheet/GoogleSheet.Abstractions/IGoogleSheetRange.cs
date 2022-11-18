namespace GoogleSheet.Abstractions
{
    public interface IGoogleSheetRange
    {
        public string? List { get; set; }
        public int StartRow { get; set; }
        public int EndRow { get; set; }
        public string? StartColumn { get; set; }
        public string? EndColumn { get; set; }
        public string ToString();
    }
}