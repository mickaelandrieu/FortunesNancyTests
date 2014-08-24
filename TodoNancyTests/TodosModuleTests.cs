namespace TodoNancyTests
{
    using CsQuery.ExtensionMethods;
    using Nancy;
    using Nancy.Testing;
    using Xunit;
    using NFluent;
    using TodoNancy;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public class TodosModuleTests
    {
        private Browser sut;
        private Todo aTodo;
        private Todo anEditedTodo;

        public TodosModuleTests()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017/todos");
            var database = mongoClient.GetServer().GetDatabase("todos");
            database.Drop();

            sut = new Browser(new Bootstrapper());
            aTodo = new Todo{ title = "task 1", order = 0, completed = false };
            anEditedTodo = new Todo{ id = 42, title = "edited title", order = 0, completed = false };
        }

        [Fact]
        public void ShouldReturnEmptyListOnGetWhenNoTodosHaveBeenPosted()
        {
            var actual = sut.Get("/todos", with => with.Accept("application/json"));

            Check.That(HttpStatusCode.OK).Equals(actual.StatusCode);
            Check.That(actual.Body.DeserializeJson<Todo[]>()).IsEmpty();
        }

        [Fact]
        public void ShouldReturnCreateHttpCodeWhenATodoIsPosted()
        {
            var actual = sut.Post("/todos/", with => with.JsonBody(aTodo));

            Check.That(HttpStatusCode.Created).Equals(actual.StatusCode);
        }

        [Fact]
        public void ShouldNotAcceptPostingToDuplicateId()
        {
            var actual = sut.Post("/todos/", with => with.JsonBody(anEditedTodo))
                            .Then
                            .Post("/todos/", with => with.JsonBody(anEditedTodo));

            Check.That(HttpStatusCode.NotAcceptable).Equals(actual.StatusCode);
        }

        [Fact]
        public void ShouldBeAbleToGetPostedTodo()
        {
            var actual = sut.Post("/todos/", with => with.JsonBody(aTodo))
                            .Then
                            .Get("/todos/", with => with.Accept("application/json"));
            var actualBody = actual.Body.DeserializeJson<Todo[]>();

            Check.That(1).Equals(actualBody.Length);
            CheckAreSame(aTodo, actualBody[0]);
        }

        [Fact]
        public void ShouldBeAbleToEditTodoWithPut()
        {
            var actual = sut.Post("/todos/", with => with.JsonBody(aTodo))
                            .Then
                            .Put("/todos/1", with => with.JsonBody(anEditedTodo))
                            .Then
                            .Get("/todos/", with => with.Accept("application/json"));

            var actualBody = actual.Body.DeserializeJson<Todo[]>();

            Check.That(actualBody.Length).Equals(1);
            CheckAreSame(anEditedTodo, actualBody[0]);
        }

        [Fact]
        public void ShouldBeAbleToDeleteTodoWithDelete()
        {
            var actual = sut.Post("/todos/", with => with.JsonBody(aTodo))
                            .Then
                            .Delete("/todos/1")
                            .Then
                            .Get("/todos/", with => with.Accept("application/json"));

            Check.That(HttpStatusCode.OK).Equals(actual.StatusCode);
            Check.That(actual.Body.DeserializeJson<Todo[]>());
        }

        private void CheckAreSame(Todo expected, Todo actual)
        {
            Check.That(expected.title).Equals(actual.title);
            Check.That(expected.order).Equals(actual.order);
            Check.That(expected.completed).Equals(actual.completed);
        }

        /**
         * XML Serialization seems broken, don't know why
         */
        [Fact]
        public void ShouldBeAbleToGetPostedXmlTodo()
        {
            var actual = sut.Post("/todos/", with =>
            {
                with.XMLBody(aTodo);
                with.Accept("application/xml");
            })
            .Then
            .Get("/todos/", with =>
            {
                with.Accept("application/json");
            });

            var actualBody = actual.Body.DeserializeJson<Todo[]>();

            Check.That(actualBody.Length).Equals(1);
            CheckAreSame(aTodo, actualBody[0]);
        }

        [Fact]
        public void ShouldBeAbleToGetPostedTodoAsXml()
        {
            var actual = sut.Post("/todos/", with =>
            {
                with.XMLBody(aTodo);
                with.Accept("application/xml");
            })
            .Then
            .Get("/todos/", with => with.Accept("application/xml"));

            var actualBody = actual.Body.DeserializeXml<Todo[]>();

            Check.That(actualBody.Length).Equals(1);
            CheckAreSame(aTodo, actualBody[0]);
        }
    }
}
