using Inventex.API.Management.Domain.Services;
using Inventex.API.Management.Domain.Repositories;
using Inventex.API.Management.Domain.Models;
using Inventex.API.Management.Domain.Services.Communication;

namespace Inventex.API.Management.Services;

public class MachineService : IMachineService
{
    private readonly IMachineRepository _machineRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public MachineService(IMachineRepository machineRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _machineRepository = machineRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<Machine>> ListAsync()
    {
        return await _machineRepository.ListAsync();
    }

    public async Task<IEnumerable<Machine>> ListByUserIdAsync(int userId)
    {
        return await _machineRepository.FindByUserIdAsync(userId);
    }

    public async Task<MachineResponse> SaveAsync(Machine machine)
    {
        // Validate CategoryId

        var existingUser = await _userRepository.FindByIdAsync(machine.UserId);

        if (existingUser == null)
            return new MachineResponse("Invalid User");
        
        // Validate Title

        var existingMachineWithName = await _machineRepository.FindByNameAsync(machine.Name);

        if (existingMachineWithName != null)
            return new MachineResponse("machine title already exists.");

        try
        {
            // Add machine
            await _machineRepository.AddAsync(machine);
            
            // Complete Transaction
            await _unitOfWork.CompleteAsync();
            
            // Return response
            return new MachineResponse(machine);

        }
        catch (Exception e)
        {
            // Error Handling
            return new MachineResponse($"An error occurred while saving the machine: {e.Message}");
        }

        
    }

    public async Task<MachineResponse> UpdateAsync(int machineId, Machine machine)
    {
        var existingMachine = await _machineRepository.FindByIdAsync(machineId);
        
        // Validate machine

        if (existingMachine == null)
            return new MachineResponse("machine not found.");

        // Validate CategoryId

        var existingUser = await _userRepository.FindByIdAsync(machine.UserId);

        if (existingUser== null)
            return new MachineResponse("Invalid User");
        
        // Validate Title

        var existingMachineWithName = await _machineRepository.FindByNameAsync(machine.Name);

        if (existingMachineWithName != null && existingMachineWithName.Id != existingMachine.Id)
            return new MachineResponse("machine title already exists.");
        
        // Modify Fields
        existingMachine.Name = machine.Name;
        existingMachine.Description = machine.Description;

        try
        {
            _machineRepository.Update(existingMachine);
            await _unitOfWork.CompleteAsync();

            return new MachineResponse(existingMachine);
            
        }
        catch (Exception e)
        {
            // Error Handling
            return new MachineResponse($"An error occurred while updating the machine: {e.Message}");
        }

    }

    public async Task<MachineResponse> DeleteAsync(int machineId)
    {
        var existingMachine = await _machineRepository.FindByIdAsync(machineId);
        
        // Validate machine

        if (existingMachine == null)
            return new MachineResponse("machine not found.");
        
        try
        {
            _machineRepository.Remove(existingMachine);
            await _unitOfWork.CompleteAsync();

            return new MachineResponse(existingMachine);
            
        }
        catch (Exception e)
        {
            // Error Handling
            return new MachineResponse($"An error occurred while deleting the machine: {e.Message}");
        }

    }
}