using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
public class UsersController : BaseApiController
{

        public IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

    public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
    {
            _photoService=photoService;
            _mapper = mapper;
            _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
    {
        var currentUser=await _userRepository.GetUserByUsername(User.GetUsername());
        userParams.CurrentUsername=currentUser.UserName;

        if(string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender=currentUser.Gender=="male"?"frmaile":"male";
        }
        
        var users=await _userRepository.GetMembersAsync(userParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages)); 
        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        return await _userRepository.GetMemberAsync(username);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        // var username=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userRepository.GetUserByUsername(User.GetUsername());
        if(user==null) return NotFound();
        _mapper.Map(memberUpdateDto, user);

        if(await _userRepository.SaveAllAsync()) return NoContent();
        return BadRequest("failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user=await _userRepository.GetUserByUsername(User.GetUsername());

        if(user==null) return NotFound();

        var result=await _photoService.AddPhotoAsync(file);

        if(result.Error!=null) return BadRequest(result.Error.Message);

        var photo=new Photo
        {
            Url=result.SecureUrl.AbsoluteUri,
            PublicId=result.PublicId
        };

        if(user.Photos.Count == 0) photo.IsMain=true;

        user.Photos.Add(photo);
        
        if(await _userRepository.SaveAllAsync()) 
        {
            return CreatedAtAction(nameof(GetUser), new {username= user.UserName}, _mapper.Map<PhotoDto>(photo)); 
        }

        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user=await _userRepository.GetUserByUsername(User.GetUsername());

        if(user==null) return NotFound();

        var photo=user.Photos.FirstOrDefault(p => p.Id==photoId);

        if(photo==null) return NotFound();

        if(photo.IsMain)  BadRequest("This is already your main photo");

        var currentMain=user.Photos.FirstOrDefault(x=>x.IsMain);
        if(currentMain!=null) currentMain.IsMain=false;
        photo.IsMain=true;

        if(await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Problem setting the main photo");
    }


    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user=await _userRepository.GetUserByUsername(User.GetUsername());
        var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);
        if(photo==null) return NotFound();
        if(photo.IsMain) return BadRequest("You cannot delete main photo");
        if(photo.PublicId!=null)
        {
            var result=await _photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error!=null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if(await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting photo");
    }

}
}