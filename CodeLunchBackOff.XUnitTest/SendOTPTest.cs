using CodeLunchBackOff.Controllers;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CodeLunchBackOff.XUnitTest
{
    public class SendOTPTest
    {
        private readonly Mock<ICounter> counter;
        private readonly Mock<IBackOff> dac;
        private readonly Mock<ITime> time;
        private readonly Mock<IOTP> otp;

        public SendOTPTest()
        {
            this.counter = new Mock<ICounter>();
            this.dac = new Mock<IBackOff>();
            this.time = new Mock<ITime>();
            this.otp = new Mock<IOTP>();
        }


        public class SendOTPNonBackOffData
        {
            public static IEnumerable<object[]> Data =>
                new List<object[]>
                {
                    new object[] { 0 ,
                         new BackOff { UnlockedTime = new DateTime(2019, 10, 20, 0, 0, 0) },
                         new DateTime(2020, 10, 20, 0, 5, 0),
                         "45678",
                         new SendOTPResponse{ Code ="45678",ReqAttempt = 1, ShouldSendOTP = true ,
                             UnlockedTime =  new DateTime(2020, 10, 20, 0, 6, 0) },},
                    new object[] { 1 ,
                         new BackOff { UnlockedTime = new DateTime(2019, 10, 20, 0, 0, 0) },
                         new DateTime(2020, 10, 20, 0, 5, 0),
                         "45678",
                         new SendOTPResponse{ Code ="45678",ReqAttempt = 2, ShouldSendOTP = true ,
                             UnlockedTime =  new DateTime(2020, 10, 20, 0, 6, 0) },},
                    new object[] { 2 ,
                         new BackOff { UnlockedTime = new DateTime(2019, 10, 20, 0, 0, 0) },
                         new DateTime(2020, 10, 20, 0, 5, 0),
                         "45678",
                         new SendOTPResponse{ Code ="45678",ReqAttempt = 3, ShouldSendOTP = true ,
                             UnlockedTime =  new DateTime(2020, 10, 20, 0, 6, 0) },},
                     new object[] { 3 ,
                         new BackOff { UnlockedTime = new DateTime(2019, 10, 20, 0, 0, 0) },
                         new DateTime(2020, 10, 20, 0, 5, 0),
                         "45678",
                         new SendOTPResponse{ Code ="45678",ReqAttempt = 4, ShouldSendOTP = true ,
                             UnlockedTime =  new DateTime(2020, 10, 20, 0, 35, 0) },},
                     new object[] { 4 ,
                         new BackOff { UnlockedTime = new DateTime(2019, 10, 20, 0, 0, 0) },
                         new DateTime(2020, 10, 20, 0, 5, 0),
                         "45678",
                         new SendOTPResponse{ Code ="45678",ReqAttempt = 5, ShouldSendOTP = true ,
                             UnlockedTime =  new DateTime(2020, 10, 20, 0, 35, 0) },},
                     new object[] { 5 ,
                         new BackOff { UnlockedTime = new DateTime(2019, 10, 20, 0, 0, 0) },
                         new DateTime(2020, 10, 20, 0, 5, 0),
                         "45678",
                         new SendOTPResponse{ Code ="45678",ReqAttempt = 6, ShouldSendOTP = true ,
                             UnlockedTime =  new DateTime(2020, 10, 20, 0, 35, 0) },},
                };
        }

        [Theory(DisplayName = "SendOTPNonBackOff")]
        [MemberData(nameof(SendOTPNonBackOffData.Data), MemberType = typeof(SendOTPNonBackOffData))]
        public void SendOTPNonBackOff(int otpCount, BackOff backOff, DateTime dateTime, string code, SendOTPResponse expected)
        {
            this.counter.Setup(x => x.GetOTP()).Returns(otpCount);
            this.dac.Setup(x => x.GetBackOff()).Returns(backOff);
            this.time.Setup(x => x.GetNow()).Returns(dateTime);
            this.otp.Setup(x => x.GenerateOTP()).Returns(code);

            BackOffController backOffCtr = new BackOffController(counter.Object, dac.Object, time.Object, otp.Object);
            var result = backOffCtr.SendOTP();

            result.Should().BeEquivalentTo(expected);
        }

        public class SendOTPBackOffData
        {
            public static IEnumerable<object[]> Data =>
                new List<object[]>
                {
                    new object[] { 0 ,
                         new BackOff { UnlockedTime = new DateTime(2020, 10, 20, 0, 0, 0) },
                         new DateTime(2019, 10, 20, 0, 5, 0),
                         "45678",
                         new SendOTPResponse{ Code = null,ReqAttempt = 0, ShouldSendOTP = false ,
                             UnlockedTime =  new DateTime(2020, 10, 20, 0, 0, 0) },},
                     new object[] { 5 ,
                         new BackOff { UnlockedTime = new DateTime(2020, 10, 20, 0, 0, 0) },
                         new DateTime(2019, 10, 20, 0, 35, 0),
                         "45678",
                         new SendOTPResponse{ Code = null ,ReqAttempt = 5, ShouldSendOTP = false ,
                             UnlockedTime =  new DateTime(2020, 10, 20, 0, 0, 0) },},
                };
        }
        [Theory(DisplayName = "SendOTPBackOff")]
        [MemberData(nameof(SendOTPBackOffData.Data), MemberType = typeof(SendOTPBackOffData))]
        public void SendOTPBackOff(int otpCount, BackOff backOff, DateTime dateTime, string code, SendOTPResponse expected)
        {
            this.counter.Setup(x => x.GetOTP()).Returns(otpCount);
            this.dac.Setup(x => x.GetBackOff()).Returns(backOff);
            this.time.Setup(x => x.GetNow()).Returns(dateTime);
            this.otp.Setup(x => x.GenerateOTP()).Returns(code);

            BackOffController backOffCtr = new BackOffController(counter.Object, dac.Object, time.Object, otp.Object);
            var result = backOffCtr.SendOTP();

            result.Should().BeEquivalentTo(expected);
        }
    }
}
