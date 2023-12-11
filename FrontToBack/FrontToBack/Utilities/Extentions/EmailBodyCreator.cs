using FrontToBack.DAL;
using FrontToBack.Interfaces;
using FrontToBack.Models;
using Microsoft.AspNetCore.Identity;

namespace FrontToBack.Utilities.Extentions
{
    public static class EmailBodyCreator
    {
        
        public static string EmailBody(AppUser appUser )
        {
            string emailbody = @"<table style=""width: 100%; border-collapse: collapse; margin-top: 20px;"">
                                 <thead>
                                     <tr>
                                         <th style = ""background-color: #4CAF50; color: white; padding: 12px; text-align: left; border-bottom: 1px solid #ddd;""> Name </th>
                                         <th style = ""background-color: #4CAF50; color: white; padding: 12px; text-align: left; border-bottom: 1px solid #ddd;""> Price </th>
                                         <th style = ""background-color: #4CAF50; color: white; padding: 12px; text-align: left; border-bottom: 1px solid #ddd;""> Count </th>
                                     </tr>
                                 </thead>
                                 <tbody>";
            foreach (var item in appUser.BasketItems)
            {
                emailbody += $@" <tr style = ""background-color: #f9f9f9;"">
                                     <td style = ""padding: 12px; text-align: left; border-bottom: 1px solid #ddd;""> {item.Product.Name} </td>
                                     <td style = ""padding: 12px; text-align: left; border-bottom: 1px solid #ddd;"">${item.Price}</td>
                                     <td style = ""padding: 12px; text-align: left; border-bottom: 1px solid #ddd;""> {item.Count} </td>
                                     
                                 </tr>";
            }
            emailbody += $@"
                           <th style = ""background-color: #4CAF50; color: white; padding: 12px; text-align: left; border-bottom: 1px solid #ddd;""> Total: {appUser.BasketItems.Sum(bi => bi.Price)}</th>
                               </tbody>
                           </table>";
            return emailbody;
        }
    }
}
