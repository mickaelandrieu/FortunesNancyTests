namespace TodoNancyTests
{
    using Nancy;
    using Nancy.Testing;
    using Xunit;
    using NFluent;

    public class HomeModuleTests
    {
        [Fact]
        public void HomePageIsReachable()
        {
            var sut = new Browser(new DefaultNancyBootstrapper());
            var actual = sut.Get("/");

            Check.That(HttpStatusCode.OK).Equals(actual.StatusCode);
        }
    }
}
