using System;
using System.Collections.Generic;

namespace WorkersAccessTest.Core {
    public static class AccessManager {
        private const string PersonsTableName = "Persons";
        private const string PermissionsTableName = "Permissions";
        private const string SectorsTableName = "Sectors";
        private const string RoomsTableName = "Rooms";
        private const string PersonIdColumnName = "PersonId";
        private const string SectorIdColumnName = "SectorId";
        private const string RoomIdColumnName = "RoomId";
        private const string IdColumnName = "ID";

        public static bool HasPermissionById(string personId, string sectorId, string roomId) {
            if (SqlManager.ExecuteAndGet(BuildHasPermissionQuery(personId, sectorId, roomId)).Count > 0)
                return true;
            return false;
        }

        public static void AddPersonPermissionById(string personId, string sectorId, string roomId) {
            if (!IsPersonExistsById(personId) || !IsSectorExistsById(sectorId) ||
                !IsRoomExistsById(sectorId, roomId)) return;
            if (HasPermissionById(personId, sectorId, roomId))
                return;
            SqlManager.Execute(BuildAddPermissionQuery(personId, sectorId, roomId));
        }

        public static void AddSectorPermissionById(string sectorIdToAdd, string sectorId, string roomId) {
            if (!IsSectorExistsById(sectorIdToAdd) || !IsSectorExistsById(sectorId) ||
                !IsRoomExistsById(sectorId, roomId)) return;
            List<List<String>> persons = GetPersonsFromSector(sectorIdToAdd);
            foreach (List<string> person in persons) {
                AddPersonPermissionById(person[0], sectorId, roomId);
            }
        }

        private static bool IsPersonExistsById(string personId) {
            return SqlManager.ExecuteAndGet(BuildExistsPersonByIdQuery(personId)).Count > 0;
        }

        private static bool IsSectorExistsById(string sectorId) {
            return SqlManager.ExecuteAndGet(BuildExistsSectorByIdQuery(sectorId)).Count > 0;
        }

        private static bool IsRoomExistsById(string sectorId, string roomId) {
            if (!IsSectorExistsById(sectorId))
                return false;
            return SqlManager.ExecuteAndGet(BuildExistsRoomByIdQuery(sectorId, roomId)).Count > 0;
        }

        private static List<List<string>> GetPersonsFromSector(string sectorId) {
            return SqlManager.ExecuteAndGet(BuildGetPersonsFromSectorQuery(sectorId));
        }

        private static string BuildExistsSectorByIdQuery(string sectorId) {
            return "Select * from " + SectorsTableName + " Where (" + IdColumnName + " = " + sectorId + ")";
        }

        private static string BuildExistsPersonByIdQuery(string personId) {
            return "Select * from " + PersonsTableName + " Where (" + IdColumnName + " = " + personId + ")";
        }

        private static string BuildExistsRoomByIdQuery(string sectorId, string roomId) {
            return "Select * from " + RoomsTableName + " Where (" + SectorIdColumnName + " = " + sectorId + " and " +
                   IdColumnName + " = " + roomId + ")";
        }

        private static string BuildGetPersonsFromSectorQuery(string sectorId) {
            return "Select " + IdColumnName + " from " + PersonsTableName + " Where (" + SectorIdColumnName + "=" +
                   sectorId + ")";
        }

        private static string BuildHasPermissionQuery(string personId, string sectorId, string roomId) {
            return "Select * from " + PermissionsTableName + " Where (" + PersonIdColumnName + " = " + personId +
                   " and " + SectorIdColumnName + "=" + sectorId + " and " + RoomIdColumnName + " = " + roomId + ")";
        }

        private static string BuildAddPermissionQuery(string personId, string sectorId, string roomId) {
            return "Insert into " + PermissionsTableName + "(" + PersonIdColumnName + "," + SectorIdColumnName + "," +
                   RoomIdColumnName + ") values(" + personId + "," + sectorId + "," + roomId + ")";
        }
    }
}