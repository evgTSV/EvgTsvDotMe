module EvgTsvDotMe.PagesProviderTests

open EvgTsvDotMe.PageResolving
open NUnit.Framework

type TestPagesProvider = PagesProvider<PagesDir="View/Pages">

[<Test>]
let ``Home page file in pages directory`` () =
    Assert.That(TestPagesProvider.Home.Path, Is.EqualTo("/pages/home"))

[<Test>]
let ``Page file is located in sub directory``() =
    Assert.That(TestPagesProvider.Blog'.Article.Path, Is.EqualTo("/pages/blog/article"))
