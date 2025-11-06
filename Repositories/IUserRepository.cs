namespace RNStore.Repositories;

using RNStore.Models;

public interface IUserRepository
{
    public User LoginUser(Login model);
}