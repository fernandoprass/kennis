using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces;

public interface IBuilderService
{
   Task BuildAsync(AppSettings appSettings);
}
