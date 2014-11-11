namespace FortunesNancyTests
{
    using Nancy;
    using Nancy.Testing;
    using Xunit;
    using NFluent;
    using FakeItEasy;
    using FortunesNancy;

    public class DataStoreTests
    {
        private readonly IDataStore fakeDataStore;
        private Browser sut;
        private readonly Fortune aFortune;

        public DataStoreTests()
        {
            fakeDataStore = A.Fake<IDataStore>();
            sut = new Browser(with =>
            {
                with.Dependency(fakeDataStore);
                with.Module<FortunesModule>();
            });

            aTodo = new Todo() { id = 5, title = "task 10", order = 100, completed = true };
        }

        [Fact]
        public void ShouldStorePostedTodosInDatastore()
        {
            sut.Post("/todos/", with => with.JsonBody(aTodo));

            //@todo: create the correct test
        }
    }
}
