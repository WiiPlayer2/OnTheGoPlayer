using FluentAssertions;
using NUnit.Framework;
using OnTheGoPlayer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheGoPlayer.Test.Helpers
{
    [TestFixture]
    public class OptionTest
    {
        #region Public Methods

        [Test]
        public void GetValueOrDefault_WithoutValue_ReturnsDefault()
        {
            var option = Option<string>.None;

            var result = option.GetValueOrDefault();

            result.Should().Be(null);
        }

        [Test]
        public void GetValueOrDefault_WithValue_ReturnsValue()
        {
            var option = "testing".ToOption();

            var result = option.GetValueOrDefault();

            result.Should().Be("testing");
        }

        [Test]
        public void GetValueOrThrow_WithoutValue_ThrowsException()
        {
            var option = Option<string>.None;

            Action act = () => option.GetValueOrThrow();

            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void GetValueOrThrow_WithValue_ReturnsValue()
        {
            var option = "testing".ToOption();

            var result = option.GetValueOrThrow();

            result.Should().Be("testing");
        }

        [Test]
        public void IsNone_WithoutValue_ReturnsTrue()
        {
            var option = Option<string>.None;

            option.IsNone.Should().BeTrue();
        }

        [Test]
        public void IsNone_WithValue_ReturnsFalse()
        {
            var option = "testing".ToOption();

            option.IsNone.Should().BeFalse();
        }

        [Test]
        public void IsSome_WithoutValue_ReturnsFalse()
        {
            var option = Option<string>.None;

            option.IsSome.Should().BeFalse();
        }

        [Test]
        public void IsSome_WithValue_ReturnsTrue()
        {
            var option = "testing".ToOption();

            option.IsSome.Should().BeTrue();
        }

        [Test]
        public void Map_WithoutValue_ReturnsNone()
        {
            var option = Option<string>.None;

            var result = option.Map(_ => true);

            result.IsNone.Should().BeTrue();
        }

        [Test]
        public void Map_WithValue_ReturnsSome()
        {
            var option = "testing".ToOption();

            var result = option.Map(_ => true);

            result.IsSome.Should().BeTrue();
        }

        [Test]
        public void Match_WithoutValue_ReturnsOnNoneResult()
        {
            var option = Option<string>.None;

            var result = option.Match(_ => true, () => false);

            result.Should().BeFalse();
        }

        [Test]
        public void Match_WithValue_ReturnsOnSomeResult()
        {
            var option = "testing".ToOption();

            var result = option.Match(_ => true, () => false);

            result.Should().BeTrue();
        }

        #endregion Public Methods
    }
}