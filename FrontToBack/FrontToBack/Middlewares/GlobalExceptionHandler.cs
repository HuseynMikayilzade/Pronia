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
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}
