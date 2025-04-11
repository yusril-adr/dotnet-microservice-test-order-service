using DotNetService.Domain.User.Repositories;
using DotNetService.Infrastructure.Exceptions;
using DotNetService.Domain.User.Dtos;
using DotNetService.Infrastructure.Dtos;
using DotNetService.Domain.User.Messages;

namespace DotNetService.Domain.User.Services
{
    public class UserService(
        UserQueryRepository userQueryRepository,
        UserStoreRepository userStoreRepository
    )
    {
        private readonly UserQueryRepository _userQueryRepository = userQueryRepository;
        private readonly UserStoreRepository _userStoreRepository = userStoreRepository;

        public async Task<PaginationModel<UserResultDto>> Index(UserQueryDto query)
        {
            var result = await _userQueryRepository.Pagination(query);
            var formatedResult = UserResultDto.MapRepo(result.Data);
            var paginate = PaginationModel<UserResultDto>.Parse(formatedResult, result.Count, query);
            return paginate;
        }


        public async Task Create(UserCreateDto dataCreate)
        {
            var isEmailExist = await _userQueryRepository.IsEmailExists(dataCreate.Email);

            if (isEmailExist)
            {
                throw new UnprocessableEntityException(UserErrorMessage.ErrEmailAlreadyExist);
            }

            var data = UserCreateDto.Assign(dataCreate);
            await _userStoreRepository.Create(data, dataCreate.RoleIds);
        }

        public async Task<Models.User> Detail(Guid id)
        {
            var user = await _userQueryRepository.FindOneById(id);

            if (user == null)
            {
                throw new DataNotFoundException(UserErrorMessage.ErrUserNotFound);
            }

            return user;
        }

        public async Task Update(Guid id, UserUpdateDto dataUpdate)
        {
            var isEmailExist = await _userQueryRepository.IsEmailExistsExceptId(dataUpdate.Email, id);

            if (isEmailExist)
            {
                throw new UnprocessableEntityException(UserErrorMessage.ErrEmailAlreadyExist);
            }

            var data = UserUpdateDto.Assign(dataUpdate);
            await _userStoreRepository.Update(id, data, dataUpdate.RoleIds);
        }

        public async Task Delete(Guid id)
        {
            await _userStoreRepository.Delete(id);
        }
    }
}