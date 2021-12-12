# Ardin.WebRequest.Core

You can have web request such as post , get and ... easily by this package ...

nstall via NuGet
-----------------
To install Ardin.WebRequest.Core, run the following command in the Package Manager Console:

```
PM> Install-Package Ardin.WebRequest.Core
```

You can also view the [package page](https://www.nuget.org/packages/Ardin.WebRequest.Core/) on NuGet.



Usage : 

    Ardin.WebRequest webRequest = new Ardin.WebRequest();
    Ardin.Request request = new Ardin.Request()
    {
        Url = "Url",
        Body = JsonData,
        Cookie = new CookieCollection(),
        Header = new WebHeaderCollection(),
        Method = "POST"
    };
    var response = webRequest.MakeRequest(request);
