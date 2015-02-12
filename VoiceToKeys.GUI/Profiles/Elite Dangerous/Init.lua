function triggerKey(command, commandVKey)
	api:Log("{0}", command);
	api:KeyDown(commandVKey);
	api:Wait(16);
	api:KeyUp(commandVKey);
end

function hardpoints(command)
	triggerKey(command, virtualKey.KEY_U);
end

function cycleNextShip(command)
	triggerKey(command, virtualKey.KEY_G);
end

function cycleNextHostile(command)
	triggerKey(command, virtualKey.KEY_H);
end

function cycleNextSubsystem(command)
	triggerKey(command, virtualKey.KEY_J);
end

function setSpeedOff(command)
	triggerKey(command, virtualKey.NUMPAD0);
end

function setSpeed025(command)
	triggerKey(command, virtualKey.NUMPAD1);
end

function setSpeed050(command)
	triggerKey(command, virtualKey.NUMPAD3);
end

function setSpeed075(command)
	triggerKey(command, virtualKey.NUMPAD7);
end

function setSpeedMax(command)
	triggerKey(command, virtualKey.NUMPAD9);
end

function landingGear(command)
	triggerKey(command, virtualKey.KEY_L);
end

function panelTarget(command)
	triggerKey(command, virtualKey.KEY_1);
end

function panelSystem(command)
	triggerKey(command, virtualKey.KEY_4);
end

function panelGalaxy(command)
	triggerKey(command, virtualKey.KEY_M);
end

function useChaffLauncher(command)
	triggerKey(command, virtualKey.KEY_C);
end

function useShieldCell(command)
	triggerKey(command, virtualKey.KEY_B);
end

function deployHeatSink(command)
	triggerKey(command, virtualKey.KEY_V);
end

-- api:AddProcessName("EliteDangerous32.exe");

local useCommand = api:AddCommand("Use");

api:AddCommand("Shield Cell", useCommand, useShieldCell);
api:AddCommand("Chaff Launcher", useCommand, useChaffLauncher);

local panelCommand = api:AddCommand("Panel");

api:AddCommand("System", panelCommand, panelSystem);
api:AddCommand("Target", panelCommand, panelTarget);
api:AddCommand("Galaxy", panelCommand, panelGalaxy);

local setSpeedCommand = api:AddCommand("Set Speed");

api:AddCommand("Off", setSpeedCommand, setSpeedOff);
api:AddCommand("25%", setSpeedCommand, setSpeed025);
api:AddCommand("50%", setSpeedCommand, setSpeed050);
api:AddCommand("75%", setSpeedCommand, setSpeed075);
api:AddCommand("Max", setSpeedCommand, setSpeedMax);

local cycleComand = api:AddCommand("Cycle");
local cycleNextCommand = api:AddCommand("Next", cycleComand);

api:AddCommand("Ship", cycleNextCommand, cycleNextShip);
api:AddCommand("Hostile", cycleNextCommand, cycleNextHostile);
api:AddCommand("Subsystem", cycleNextCommand, cycleNextSubsystem);

api:AddCommand("Hardpoints", nil, hardpoints);
api:AddCommand("Landing Gear", nil, landingGear);
api:AddCommand("Deploy Heat Sink", nil, deployHeatSink);