using CodeLunchBackOff.Controllers;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CodeLunchBackOff.XUnitTest
{
    public class VertifyOTPTest
    {
        private readonly Mock<ICounter> counter;
        private readonly Mock<IBackOff> dac;
        private readonly Mock<ITime> time;
        private readonly Mock<IOTP> otp;

        public VertifyOTPTest()
        {
            this.counter = new Mock<ICounter>();
            this.dac = new Mock<IBackOff>();
            this.time = new Mock<ITime>();
            this.otp = new Mock<IOTP>();
        }

        public class VertifyFailData
        {
            public static IEnumerable<object[]> Data =>
                new List<object[]>
                {
                      new object[] { "123456", "778899", 0, new VerifyConfig { MaxVetifyCount = 7},new VertifyResponse { IsPassed = false, Remark = string.Empty, ShouldCheck = true, VerAttempt =1 }, },
                      new object[] { "123456", "778899", 1, new VerifyConfig { MaxVetifyCount = 7},new VertifyResponse { IsPassed = false, Remark = string.Empty, ShouldCheck = true, VerAttempt =2 }, },
                      new object[] { "123456", "778899", 2, new VerifyConfig { MaxVetifyCount = 7},new VertifyResponse { IsPassed = false, Remark = string.Empty, ShouldCheck = true, VerAttempt =3 }, },
                      new object[] { "123456", "778899", 3, new VerifyConfig { MaxVetifyCount = 7},new VertifyResponse { IsPassed = false, Remark = string.Empty, ShouldCheck = true, VerAttempt =4 }, },
                      new object[] { "123456", "778899", 4, new VerifyConfig { MaxVetifyCount = 7},new VertifyResponse { IsPassed = false, Remark = string.Empty, ShouldCheck = true, VerAttempt =5 }, },
                      new object[] { "123456", "778899", 5, new VerifyConfig { MaxVetifyCount = 7},new VertifyResponse { IsPassed = false, Remark = string.Empty, ShouldCheck = true, VerAttempt =6 }, },
                      new object[] { "123456", "778899", 6, new VerifyConfig { MaxVetifyCount = 7},new VertifyResponse { IsPassed = false, Remark = string.Empty, ShouldCheck = true, VerAttempt =7 }, },
                      new object[] { "123456", "778899", 7, new VerifyConfig { MaxVetifyCount = 7},new VertifyResponse { IsPassed = false, Remark = "Attempts count are over 7 times", ShouldCheck = false, VerAttempt =7 }, },
                      new object[] { "778899", "778899", 7, new VerifyConfig { MaxVetifyCount = 7},new VertifyResponse { IsPassed = false, Remark = "Attempts count are over 7 times", ShouldCheck = false, VerAttempt =7 }, },


                };
        }
        [Theory(DisplayName = " VertifyFail")]
        [MemberData(nameof(VertifyFailData.Data), MemberType = typeof(VertifyFailData))]
        public void VertifyFail(string code, string validCode, int lastestAttempt, VerifyConfig verifyConfig, VertifyResponse expected)
        {
            this.otp.Setup(x => x.GetOTP()).Returns(validCode);
            this.counter.Setup(x => x.GetVertify()).Returns(lastestAttempt);
            this.dac.Setup(x => x.GetVerifyConfig()).Returns(verifyConfig);

            BackOffController backOffCtr = new BackOffController(counter.Object, dac.Object, time.Object, otp.Object);
            var result = backOffCtr.Vertify(code);

            result.Should().BeEquivalentTo(expected);
        }

        public class VertifyPassData
        {
            public static IEnumerable<object[]> Data =>
                new List<object[]>
                {
                      new object[] { "778899", "778899", 1, new VerifyConfig { MaxVetifyCount = 7},new VertifyResponse { IsPassed = true, Remark = string.Empty, ShouldCheck = true, VerAttempt =0 }, },
                      new object[] { "778899", "778899", 7, new VerifyConfig { MaxVetifyCount = 7},new VertifyResponse { IsPassed = false, Remark = "Attempts count are over 7 times", ShouldCheck = false, VerAttempt =7 }, },
                };
        }
        [Theory(DisplayName = " VertifyPass")]
        [MemberData(nameof(VertifyPassData.Data), MemberType = typeof(VertifyPassData))]
        public void VertifyPass(string code, string validCode, int lastestAttempt, VerifyConfig verifyConfig, VertifyResponse expected)
        {
            this.otp.Setup(x => x.GetOTP()).Returns(validCode);
            this.counter.Setup(x => x.GetVertify()).Returns(lastestAttempt);
            this.dac.Setup(x => x.GetVerifyConfig()).Returns(verifyConfig);

            BackOffController backOffCtr = new BackOffController(counter.Object, dac.Object, time.Object, otp.Object);
            var result = backOffCtr.Vertify(code);

            result.Should().BeEquivalentTo(expected);
        }
    }
}
