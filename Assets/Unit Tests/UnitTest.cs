﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class UnitTest 
{
    private Game game;
    private Map map;
	private Player[] players;
	private PlayerUI[] gui;
    private GameObject unitPrefab;

    private void Setup()
    {
        TestSetup t = new TestSetup();
        this.game = t.GetGame();
        this.map = t.GetMap();
        this.players = t.GetPlayers();
        this.gui = t.GetPlayerUIs();
        this.unitPrefab = t.GetUnitPrefab();
    }

    [UnityTest]
    public IEnumerator MoveToFriendlyFromNull_UnitInCorrectSector() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];

        // test moving from null
        unit.SetSector(null);
        sectorA.SetUnit(null);
        unit.SetOwner(playerA);
        sectorA.SetOwner(playerA);

        unit.MoveTo(sectorA);
        Assert.IsTrue(unit.GetSector() == sectorA);
        Assert.IsTrue(sectorA.GetUnit() == unit);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveToNeutral_UnitInCorrectSector() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];
        Player playerA = players[0];

        // test moving from one sector to another
        unit.SetSector(sectorA);
        unit.SetOwner(playerA);
        sectorA.SetUnit(unit);
        sectorB.SetUnit(null);
        sectorA.SetOwner(playerA);
        sectorB.SetOwner(playerA);

        unit.MoveTo(sectorB);
        Assert.IsTrue(unit.GetSector() == sectorB);
        Assert.IsTrue(sectorB.GetUnit() == unit);
        Assert.IsNull(sectorA.GetUnit());

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveToFriendly_UnitInCorrectSector() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];

        // test moving into a friendly sector (no level up)
        unit.SetLevel(1);
        unit.SetSector(null);
        sectorA.SetUnit(null);
        unit.SetOwner(playerA);
        sectorA.SetOwner(playerA);

        unit.MoveTo(sectorA);
        Assert.IsTrue(unit.GetLevel() == 1);

        yield return null;
    }

    public IEnumerator MoveToHostile_UnitInCorrectSectorAndLevelUp() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];
        Player playerB = players[1];

        // test moving into a non-friendly sector (level up)
        unit.SetLevel(1);
        unit.SetSector(null);
        sectorA.SetUnit(null);
        unit.SetOwner(playerA);
        sectorA.SetOwner(playerB);

        unit.MoveTo(sectorA);
        Assert.IsTrue(unit.GetLevel() == 2);
        Assert.IsTrue(sectorA.GetOwner() == unit.GetOwner());

        yield return null;
    }

    [UnityTest]
    public IEnumerator SwapPlaces_UnitsInCorrectNewSectors() {

        Setup();

        Unit unitA = map.sectors[0].GetUnit();
        Unit unitB = map.sectors[1].GetUnit();
        
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        yield return null;

        unitA.SwapPlacesWith(unitB);
        Assert.IsTrue(unitA.GetSector() == sectorB); // unitA in sectorB
        Assert.IsTrue(sectorB.GetUnit() == unitA); // sectorB has unitA
        Assert.IsTrue(unitB.GetSector() == sectorA); // unitB in sectorA
        Assert.IsTrue(sectorA.GetUnit() == unitB); // sectorA has unitB

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelUp_UnitLevelIncreasesByOne() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();

        // ensure LevelUp increments level as expected
        unit.SetLevel(1);
        unit.LevelUp();
        Assert.IsTrue(unit.GetLevel() == 2);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelUp_UnitLevelDoesNotPastFive() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();

        // ensure LevelUp does not increment past 5
        unit.SetLevel(5);
        unit.LevelUp();
        Assert.IsTrue(unit.GetLevel() == 5);

        yield return null;
    }
        
    [UnityTest]
    public IEnumerator SelectAndDeselect_SelectedTrueWhenSelectedFalseWhenDeselected() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sector = map.sectors[0];

        unit.SetSector(sector);
        unit.SetSelected(false);

        unit.Select();
        Assert.IsTrue(unit.IsSelected());

        unit.Deselect();
        Assert.IsFalse(unit.IsSelected());

        yield return null;
    }


    [UnityTest]
    public IEnumerator DestroySelf_UnitNotInSectorAndNotInPlayersUnitsList() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sector = map.sectors[0];
        Player player = players[0];

        unit.SetSector(sector);
        sector.SetUnit(unit);

        unit.SetOwner(player);
        player.units.Add(unit);

        unit.DestroySelf();

        Assert.IsNull(sector.GetUnit()); // unit not on sector 
        Assert.IsFalse(player.units.Contains(unit)); // unit not in list of players units

        yield return null;
    }
}