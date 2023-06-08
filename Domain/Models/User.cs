using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class User : IdentityUser
    {
        public double Balance { get; set; }
        public List<HashCrackInfo> HashCrackInfos { get; set; }
    }
}
