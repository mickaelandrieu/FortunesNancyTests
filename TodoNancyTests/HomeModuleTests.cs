namespace TodoNancyTests
{
    using Nancy;
    using Nancy.Testing;
    using Xunit;
    using NFluent;
    using TodoNancy;

    public class HomeModuleTests
    {
        [Fact]
        public void HomePageIsReachable()
        {
            var sut = new Browser(new Bootstrapper());
            var actual = sut.Get("/");

            Check.That(HttpStatusCode.OK).Equals(actual.StatusCode);
        }
    }
}
