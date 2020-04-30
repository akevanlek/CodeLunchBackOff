using CodeLunchBackOff.Controllers;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CodeLunchBackOff.XUnitTest
{
    public class GetUnLockedTimeSecoundTest
    {
        private readonly Mock<ICounter> counter;
        private readonly Mock<IBackOff> dac;
        private readonly Mock<ITime> time;
        private readonly Mock<IOTP> otp;

        public GetUnLockedTimeSecoundTest()
        {
            this.counter = new Mock<ICounter>();
            this.dac = new Mock<IBackOff>();
            this.time = new Mock<ITime>();
            this.otp = new Mock<IOTP>();
        }

        public class GetUnLockedTimeSecoundData
        {
            public static IEnumerable<object[]> Data =>
                new List<object[]>
                {
                    new object[] { 0,60},
                    new object[] { 1,60},
                    new object[] { 2,60},
                    new object[] { 3,1800},
                    new object[] { 4,1800},
                    new object[] { 5,1800},
                    new object[] { 6,1800},
                };
        }

        [Theory(DisplayName = " GetUnLockedTimeSecound ")]
        [MemberData(nameof(GetUnLockedTimeSecoundData.Data), MemberType = typeof(GetUnLockedTimeSecoundData))]
        public void GetUnLockedTimeSecound(int count, int secound)
        {
            BackOffController backOff = new BackOffController(counter.Object, dac.Object, time.Object, otp.Object);
            var result = backOff.GetUnLockedTimeSecound(count);

            result.Should().Be(secound); ;
        }

    }
}
