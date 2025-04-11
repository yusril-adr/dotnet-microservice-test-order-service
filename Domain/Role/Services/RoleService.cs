using DotNetService.Domain.Role.Dtos;
using DotNetService.Domain.Role.Messages;
using DotNetService.Domain.Role.Repositories;
using DotNetService.Infrastructure.Dtos;
using DotNetService.Infrastructure.Exceptions;

namespace DotNetService.Domain.Role.Services
{
    public class RoleService(
        RoleStoreRepository roleStoreRepository,
        RoleQueryRepository roleQueryRepository
    )
    {
        private readonly RoleStoreRepository _roleStoreRepository = roleStoreRepository;
        private readonly RoleQueryRepository _roleQueryRepository = roleQueryRepository;

        public async Task<PaginationModel<RoleResultDto>> Index(RoleQueryDto query = null)
        {
            var data = await _roleQueryRepository.Pagination(query);
            var formatedData = RoleResultDto.MapRepo(data.Data);
            var paginate = PaginationModel<RoleResultDto>.Parse(formatedData, data.Count, query);
            return paginate;
        }

        public async Task Create(RoleCreateDto dataCreate)
        {
            var isRoleExist = await _roleQueryRepository.IsExistByKey(dataCreate.Key);

            if (isRoleExist)
            {
                throw new UnprocessableEntityException(RoleErrorMessage.ErrRoleAlreadyExist);
            }

            var data = RoleCreateDto.Assign(dataCreate);

            await _roleStoreRepository.Create(data, dataCreate.PermissionIds);
        }

        public async Task<RoleResultDto> DetailById(Guid id)
        {
            var role = await _roleQueryRepository.FindOneById(id);
            if (role == null)
            {
                throw new DataNotFoundException(RoleErrorMessage.ErrRoleNotFound);
            }

            return new RoleResultDto(role);
        }

        public async Task<List<Models.Role>> GetList(string search, int page, int perPage)
        {
            return await _roleQueryRepository.Get(search, page, perPage);
        }
        public async Task<int> Count(string search)
        {
            return await _roleQueryRepository.CountAll(search);
        }

        public async Task Update(Guid id, RoleUpdateDto dataUpdate)
        {
            var data = RoleUpdateDto.Assign(dataUpdate);
            await _roleStoreRepository.Update(id, data, dataUpdate.PermissionIds);
        }

        public async Task Delete(Guid id)
        {
            await _roleStoreRepository.Delete(id);
        }
    }
}