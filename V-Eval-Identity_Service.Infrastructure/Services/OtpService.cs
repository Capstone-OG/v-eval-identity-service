using System.Security.Cryptography;
using Application.Common.Interfaces;

namespace Infrastructure.Services;

public class OtpService : IOtpService
{
    public string GenerateOtpCode()
    {
        // 6 chữ số ngẫu nhiên: 100000 -> 999999
        var number = RandomNumberGenerator.GetInt32(100000, 1000000);
        return number.ToString();
    }
}
