using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

public interface IAccountService
{
    //Task<IActionResult> ConfirmEmailAsync(ConfirmEmailDTO confirmationDTO);
    Task<IEnumerable<UserDTO>> GetAll();
    Task<UserDTO> GetById(int id);
    Task<UserDTO> Update(int id, UserPutDTO user);
}