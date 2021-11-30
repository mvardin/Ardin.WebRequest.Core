# Ardin.WebRequest.Core

Use web request such as get , post , ... easily :)

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
