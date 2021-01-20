using System.Collections.Generic;

namespace Mwp.Wopi
{
    public class GetWopiFileHistoryDto
    {
        public List<WopiFileHistoryDto> FileHistories { get; set; }

        public GetWopiFileHistoryDto(List<WopiFileHistoryDto> fileHistories)
        {
            FileHistories = fileHistories;
        }
    }
}