using Data.Interfaces;
using Services.Interfaces;
using Services.Manager;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.ServiceModels;
using static Resources.Messages.ErrorMessages;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling operations related to user preferences.
    /// </summary>
    public class UserPreferencesService : IUserPreferencesService
    {
        private readonly IUserPreferencesRepository _repository;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserPreferencesService"/> class.
        /// </summary>
        /// <param name="repository">The user preferences repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public UserPreferencesService(
            IUserPreferencesRepository repository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Gets the user preferences asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. 
        /// The task result contains the user preferences view model.</returns>
        public async Task<UserPreferencesViewModel> GetUserPreferencesAsync(string userId)
        {
            var preferences = _repository.GetUserPreferences(userId);

            var model = new UserPreferencesViewModel
            {
                UserId = userId,
                Preferences = preferences,
            };
            return model;
        }

        /// <summary>
        /// Updates the user preferences asynchronously.
        /// </summary>
        /// <param name="model">The user preferences view model.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task UpdateUserPreferencesAsync(UserPreferencesViewModel model)
        {
            var existingPreferences = _repository.GetUserPreferences(model.UserId);
            if (existingPreferences != null && model.Preferences != null)
            {
                foreach (var preference in model.Preferences)
                {
                    existingPreferences[preference.Key] = preference.Value;
                }
                await _repository.UpdateUserPreferencesAsync(model.UserId, existingPreferences);
            }
        }

        /// <summary>
        /// Updates the user password.
        /// </summary>
        /// <param name="model">The user preferences view model.</param>
        /// <exception cref="InvalidOperationException">Thrown when the old password does not match, 
        /// the new password is the same as the old password, or an error occurs during the update process.</exception>
        public async Task UpdateUserPassword(UserPreferencesViewModel model)
        {
            var user = await _userRepository.GetUserByIdAsync(model.UserId);
            if (user != null)
            {
                bool isOldPasswordCorrect = IsPasswordMatch(user.Password, model.OldPassword);
                bool isNewPasswordSameAsCurrent = IsPasswordMatch(user.Password, model.NewPassword);
                if (!isOldPasswordCorrect)
                {
                    throw new InvalidOperationException(Error_UserPasswordMismatch);
                }
                if (isNewPasswordSameAsCurrent)
                {
                    throw new InvalidOperationException(Error_UserPasswordSimilar);
                }

                user.Password = PasswordManager.EncryptPassword(model.NewPassword);
                await _userRepository.UpdateUserAsync(user);
            }
            else
            {
                throw new InvalidOperationException(Error_UserPasswordUpdateDefault);
            }
        }

        private static bool IsPasswordMatch(string p1, string p2) =>
            string.Equals(p1, PasswordManager.EncryptPassword(p2));

        /// <summary>
        /// Gets a user preference by key asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="key">The preference key.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. 
        /// The task result contains the key-value pair representing the user preference.</returns>
        public async Task<KeyValuePair<string, string>> GetUserPreferenceByKeyAsync(string userId, string key) =>
            await _repository.GetUserPreferenceByKeyAsync(userId, key);
    }
}
