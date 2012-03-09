using System;
using System.Text;
using System.Web;
using NUnit.Framework;
using ScrapySharp.Utilities;

namespace ScrapySharp.Tests
{
    [TestFixture]
    public class When_using_url_utility
    {
        [Test]
        public void When_url_encoding_string_0()
        {
            Assert.That(UrlUtility.Encode(" "), Is.EqualTo("%20"));
        }

        [Test]
        public void When_url_encoding_string_1()
        {
            Assert.That(UrlUtility.Encode("\"Aardvarks lurk, OK?\""), Is.EqualTo("%22Aardvarks%20lurk%2C%20OK%3F%22"));
        }

        [Test]
        public void When_url_encoding_string_2()
        {
            Assert.That(UrlUtility.Encode("ü"), Is.EqualTo("%C3%BC"));
        }

        [Test]
        public void When_url_encoding_string_3()
        {

            Assert.That(UrlUtility.Encode("*ù;:!^;$=)=èè-'0"), Is.EqualTo("*%C3%B9%3B%3A!%5E%3B%24%3D)%3D%C3%A8%C3%A8-%270"));
        }

        [Test]
        public void When_url_decoding_string_1()
        {
            Assert.That(UrlUtility.Decode("%22Aardvarks%20lurk%2C%20OK%3F%22"), Is.EqualTo("\"Aardvarks lurk, OK?\""));
        }

        [Test]
        public void When_url_decoding_string_2()
        {
            Assert.That(UrlUtility.Decode("%C3%BC"), Is.EqualTo("ü"));
        }

        [Test]
        public void When_url_decoding_string_3()
        {
            Assert.That(UrlUtility.Decode("*%C3%B9%3B%3A!%5E%3B%24%3D)%3D%C3%A8%C3%A8-%270"), Is.EqualTo("*ù;:!^;$=)=èè-'0"));
        }

        [Test]
        public void When_detecting_decoded_string_1()
        {
            Assert.IsFalse(UrlUtility.IsEncoded("\"Aardvarks lurk, OK?\""));
        }

        [Test]
        public void When_detecting_encoded_string_1()
        {
            Assert.IsTrue(UrlUtility.IsEncoded("%22Aardvarks%20lurk%2C%20OK%3F%22"));
        }

        [Test]
        public void When_detecting_encoded_string_2()
        {
            Assert.IsFalse(UrlUtility.IsEncoded("%22Aardvarks%20lurk%2C%20OK%3F%22%0"));
        }
    }
}