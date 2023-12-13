namespace FrontToBack.Middlewares
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext Context)
        {
            try
            {
                await _next.Invoke(Context);
            }
            catch (Exception e)
            {
                //string errorurl = Path.Combine("home",$"errorpage?error={e.Message}");
                string errorurl = $"/home/errorpage?error={e.Message}";
                Context.Response.Redirect(errorurl);
            }
           
        }
    }
}
