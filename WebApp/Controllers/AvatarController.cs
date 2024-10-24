using Microsoft.AspNetCore.Mvc;
using Services.ServiceModels;
using System;
using System.Threading.Tasks;


public class AvatarController : Controller
{
    private readonly IAvatarService _avatarService;

    public AvatarController(IAvatarService avatarService)
    {
        _avatarService = avatarService;
    }

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> UploadAvatar([FromForm] UserViewModel model)
    {
        if (model.AvatarFile != null && model.AvatarFile.Length > 0)
        {
            try
            {
                var avatar = await _avatarService.UploadAvatarAsync(model.AvatarFile, model.UserId);
                TempData["SuccessMessage"] = "Avatar uploaded successfully.";
                return RedirectToAction("ProfileDetails", new { userId = model.UserId });
            }
            catch (Exception ex)
            {
                // Log the error message
                Console.WriteLine(ex.Message); // Use proper logging here
                TempData["ErrorMessage"] = "An error occurred while uploading the avatar.";
                return RedirectToAction("ProfileDetails", new { userId = model.UserId });
            }
        }

        TempData["ErrorMessage"] = "Please select a valid avatar file.";
        return RedirectToAction("Profile", new { userId = model.UserId });
    }


    [HttpPost]
    [Route("remove")]
    public async Task<IActionResult> RemoveAvatar(string avatarId)
    {
        await _avatarService.DeleteAvatarAsync(avatarId);
        TempData["SuccessMessage"] = "Avatar removed successfully.";
        return RedirectToAction("Profile", "User");
    }
}
