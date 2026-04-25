module EvgTsvDotMe.PagesProviderTests

open EvgTsvDotMe.PageResolving
open Xunit

type TestPagesProvider = PagesProvider<PagesDir="View/Pages">

[<Fact>]
let ``Home page file in pages directory`` () =
    Assert.Equal(TestPagesProvider.Home.Path, "/pages/home")

[<Fact>]
let ``Page file is located in sub directory``() =
    Assert.Equal(TestPagesProvider.Blog'.Article.Path, "/pages/blog/article")
