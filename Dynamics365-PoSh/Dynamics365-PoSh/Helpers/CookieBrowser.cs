using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dynamics365_PoSh.Helpers
{
    public partial class CookieBrowser
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);
        public void Browse(string url, CookieCollection cookies)
        {
            var form = new Form();
            var browser = new WebBrowser()
            {
                ScriptErrorsSuppressed = true,
                Dock = DockStyle.Fill
            };

            for (int i = 0; i < cookies.Count; i++)
            {
                Cookie c = cookies[i];
                InternetSetCookie(url, c.Name, c.Value);
            }

            form.SuspendLayout();
            form.Width = 900;
            form.Height = 500;
            form.Text = $"Log in to {url}";
            form.Controls.Add(browser);
            form.ResumeLayout(false);

            browser.Navigate(url);
            form.Focus();
            form.ShowDialog();
        }
    }
}
