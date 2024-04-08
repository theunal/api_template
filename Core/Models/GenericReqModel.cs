using RestSharp;

namespace Core.Models
{
    public class GenericReqModel
    {
        public string url { get; set; } = null!;
        public Method method { get; set; } = Method.Get;
        public Dictionary<string, string>? headers { get; set; }
        public Dictionary<string, string>? parameters { get; set; }
        public string? body { get; set; }
        public bool? isForm { get; set; }
        public bool? isSsl { get; set; }
        public GenericReqByteFileModel? byte_file { get; set; }
        public RetryModel? retry { get; set; }

    }

    public class RetryModel
    {
        public int Count { get; set; }
        public int Sleep { get; set; }
    }

    public class GenericReqByteFileModel
    {
        public byte[] bytes { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string Param { get; set; } = null!;
        public string? file_path { get; set; }
    }
}
