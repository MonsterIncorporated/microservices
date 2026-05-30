public interface INumberService
{
    /// <summary>
    /// Gets all generated numbers for a specific user.
    /// </summary>
    /// <param name="userId">The user from which to get the numbers.</param>
    /// <returns>All Numbers which correspond to the userid.</returns>
    public Task<ICollection<GeneratedNumberDto>> GetNumbers(Guid userId);

    /// <summary>
    /// Generates a new random number for a specific user within a given range and saves it to the database.
    /// </summary>
    /// <param name="createGeneratedNumberDto">The createGeneratedNumberDto to create.</param>
    /// <returns>The created Number.</returns>
    public Task<GeneratedNumberDto> CreateGeneratedNumberAsync(CreateGeneratedNumberDto createGeneratedNumberDto);

    /// <summary>
    /// Deletes a generated number by its id.
    /// </summary>
    /// <param name="numberId">The id of the generatedNumber to delete.</param>
    /// <returns>A Task representing the delete action.</returns>
    public Task DeleteGeneratedNumberAsync(Guid numberId);
}