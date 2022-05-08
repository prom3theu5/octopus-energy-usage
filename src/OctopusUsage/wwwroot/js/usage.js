"use strict";

const electricityMethodName = 'SendElectricityUsageAsync';
const gasMethodName = 'SendGasUsageAsync';
const gotElectricityUsageEvent = 'ElectricityUsage';
const gotGasUsageEvent = 'GasUsage';

function generateTime() {
  const today = new Date();
  return `<span style="color:forestgreen;">${today.getHours()}:${today.getMinutes()}</span>`;
}

const connection = new signalR.HubConnectionBuilder()
  .withUrl("/usage")
  .build();

connection.on(gotElectricityUsageEvent, function (usageLastDay) {
  if (usageLastDay !== null) {
    document.getElementById("electricity-usage").innerHTML = `${usageLastDay.usage} units at ${generateTime()}`;
  }
});

connection.on(gotGasUsageEvent, function (usageLastDay) {
  if (usageLastDay !== null) {
    document.getElementById("gas-usage").innerHTML = `${usageLastDay.usage} units at ${generateTime()}`;
  }
});

connection.start().then(function () {
  connection.invoke(electricityMethodName);
  connection.invoke(gasMethodName);
}).catch(function (err) {
  return console.error(err.toString());
});