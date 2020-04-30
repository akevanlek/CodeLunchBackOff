using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeLunchBackOff.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackOffController : ControllerBase
    {
        private readonly ICounter counter;
        private readonly IBackOff dac;
        private readonly ITime time;
        private readonly IOTP otp;

        public BackOffController(ICounter counter, IBackOff dac, ITime time, IOTP otp)
        {
            this.counter = counter;
            this.dac = dac;
            this.time = time;
            this.otp = otp;
        }

        [HttpGet]
        public SendOTPResponse SendOTP()
        {
            var otpCount = counter.GetOTP();
            var backOff = dac.GetBackOff();

            var now = time.GetNow();
            if (now >= backOff.UnlockedTime)
            {
                var secound = GetUnLockedTimeSecound(otpCount);

                backOff.UnlockedTime = now.AddSeconds(secound);
                dac.UpdateBackOff(backOff);

                counter.IncrementOTP();

                var code = otp.GenerateOTP();

                return new SendOTPResponse
                {
                    Code = code,
                    ReqAttempt = otpCount + 1,
                    ShouldSendOTP = true,
                    UnlockedTime = backOff.UnlockedTime
                };
            }
            else
            {
                return new SendOTPResponse
                {
                    ShouldSendOTP = false,
                    UnlockedTime = backOff.UnlockedTime,
                    ReqAttempt = otpCount
                };
            }
        }

        [HttpGet]
        public VertifyResponse Vertify(string code)
        {
            var currentCount = counter.GetVertify();
            var maxVertify = dac.GetVerifyConfig().MaxVetifyCount;

            if (currentCount >= maxVertify)
            {
                return new VertifyResponse
                {
                    IsPassed = false,
                    VerAttempt = currentCount,
                    Remark = "Attempts count are over 7 times",
                    ShouldCheck = false
                };
            }

            var codeValid = otp.GetOTP();

            if (codeValid == code)
            {
                counter.SetVertify(0);
                counter.SetOTP(0);
                return new VertifyResponse
                {
                    IsPassed = true,
                    VerAttempt = 0,
                    Remark = string.Empty,
                    ShouldCheck = true
                };
            }
            else
            {
                counter.IncrementVertify();
                return new VertifyResponse
                {
                    IsPassed = false,
                    VerAttempt = currentCount + 1,
                    Remark = string.Empty,
                    ShouldCheck = true
                };
            }
        }

        public int GetUnLockedTimeSecound(int count)
        {
            if (count >= 3)
            {
                return 1800;
            }
            else
            {
                return 60;
            }
        }
    }

    public class VertifyResponse
    {
        public bool IsPassed { get; set; }
        public int VerAttempt { get; set; }
        public bool ShouldCheck { get; set; }
        public string Remark { get; set; }
    }
    public class BackOff
    {
        public DateTime UnlockedTime { get; set; }
    }

    public class SendOTPResponse
    {
        public bool ShouldSendOTP { get; set; }

        public DateTime UnlockedTime { get; set; }
        public int ReqAttempt { get; set; }
        public string Code { get; set; }
    }

    public class VerifyConfig
    {
        public int MaxVetifyCount { get; set; }
    }

    public interface IBackOff
    {
        BackOff GetBackOff();
        void UpdateBackOff(BackOff backoff);

        BackOff CheckBackOff();

        VerifyConfig GetVerifyConfig();
    }

    public interface IOTP
    {
        string GetOTP();
        string GenerateOTP();
    }

    public interface ICounter
    {
        int GetVertify();
        void SetVertify(int count);
        void IncrementVertify();

        int GetOTP();
        void SetOTP(int count);
        void IncrementOTP();
    }

    public interface ITime
    {
        DateTime GetNow();
    }
}