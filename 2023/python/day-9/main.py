from concurrent.futures import ThreadPoolExecutor
from enum import Enum
import os
from pathlib import Path
import asyncio
import curses
import time


class Status(Enum):
    Operational = "."
    Broken = "#"
    Unknown = "?"


class StatusRow:
    Statuses: list[Status]

    # Describes the size of broken groups
    Broken_Pattern: list[int]

    def find_broken_groups(self) -> list[int]:
        broken_groups = []
        current_group = 0
        for status in self.Statuses:
            if status == Status.Broken:
                current_group += 1
            else:
                if current_group > 0:
                    broken_groups.append(current_group)
                    current_group = 0
        if current_group > 0:
            broken_groups.append(current_group)
        return broken_groups

    def find_broken_groups_start_indexes(self) -> list[int]:
        broken_groups = []
        current_group = 0
        for i in range(len(self.Statuses)):
            if self.Statuses[i] == Status.Broken:
                current_group += 1
            else:
                if current_group > 0:
                    broken_groups.append(i - current_group)
                    current_group = 0
        if current_group > 0:
            broken_groups.append(i + 1 - current_group)
        return broken_groups

    def find_unknown_groups(self) -> list[int]:
        unknown_groups = []
        current_group = 0
        for status in self.Statuses:
            if status == Status.Unknown:
                current_group += 1
            else:
                if current_group > 0:
                    unknown_groups.append(current_group)
                    current_group = 0
        return unknown_groups

    def find_unknown_group_around(self, index: int) -> (int, int):
        """Returns the start and end index of the unknown group around the given index"""
        start = index
        end = index
        for i in range(index, len(self.Statuses)):
            if self.Statuses[i] == Status.Unknown:
                end += 1
            else:
                break
        for i in range(index, -1, -1):
            if self.Statuses[i] == Status.Unknown:
                start -= 1
            else:
                break
        return (start, end)

    def get_next_missing_group_start(self) -> int:
        pattern = self.Broken_Pattern
        broken_groups = self.find_broken_groups()
        broken_groups_indexes = self.find_broken_groups_start_indexes()

        for i in range(len(broken_groups)):
            size = broken_groups[i]
            starts_at = broken_groups_indexes[i]
            if size == pattern[i]:
                continue
            elif size < pattern[i]:
                return starts_at + size

    def is_surrounded_by_unknown(self, index: int) -> bool:
        """Returns true if the given index is surrounded by unknown statuses"""
        start, end = self.find_unknown_group_around(index)
        return start < index and end > index

    def is_not_surrounded_by_broken(self, index: int) -> bool:
        """Returns true if the given index is not surrounded by broken statuses, there might be operational statuses in between"""
        nrOfStatuses = len(self.Statuses)

        if nrOfStatuses - 1 < 0 or nrOfStatuses - 1 < index or index < 0:
            return False

        if self.Statuses[index] == Status.Broken:
            return False
        if self.Statuses[index - 1] == Status.Broken:
            return False
        if nrOfStatuses - 1 <= index:
            return False
        if self.Statuses[index + 1] == Status.Broken:
            return False
        return True

    def find_next_unknown_island(self, index: int) -> int:
        """Returns the next index that is not surrounded by broken statuses"""
        free_spots = self.list_free_spots()
        for i in range(index, len(self.Statuses)):
            if free_spots[i] == index:
                return i
        return -1

    def get_next_unknown(self, index: int) -> int:
        """Returns the next index that is not surrounded by broken statuses"""
        for i in range(index, len(self.Statuses)):
            if self.Statuses[i] == Status.Unknown:
                return i
            elif self.Statuses[i] != Status.Broken:
                return -1
        return -1

    def get_previous_unknown(self, index: int) -> int:
        """Returns the next index that is not surrounded by broken statuses"""
        for i in range(index, -1, -1):
            if self.Statuses[i] == Status.Unknown:
                return i
            elif self.Statuses[i] != Status.Broken:
                return -1
        return -1

    def find_previous_unknown_island(self, index: int) -> int:
        """Returns the previous index that is not surrounded by broken statuses"""
        free_spots = self.list_free_spots()
        for i in range(index, -1, -1):
            if free_spots[i] == index:
                return i
        return -1

    def list_free_spots(self) -> list[int]:
        """Returns a list of indexes where a free spot is available"""
        free_spots = []
        for i in range(len(self.Statuses)):
            if self.is_not_surrounded_by_broken(i):
                free_spots.append(i)
        return free_spots

    def clone(self):
        clone = StatusRow()
        clone.Statuses = self.Statuses.copy()
        clone.Broken_Pattern = self.Broken_Pattern.copy()
        return clone

    def print(self):
        print(self)

    def __str__(self):
        to_print = ""
        for status in self.Statuses:
            to_print += status.value
        return to_print


status_rows: list[StatusRow] = []

for line in Path(__file__).parent.joinpath("input.txt").read_text().splitlines():
    statuses, pattern = line.split(" ")
    status_row = StatusRow()
    status_row.Statuses = [
        Status.Unknown
        if status == Status.Unknown.value
        else Status.Broken
        if status == Status.Broken.value
        else Status.Operational
        for status in statuses
    ]
    status_row.Broken_Pattern = [
        int(group) for group in pattern.split(",") if group != ""
    ]

    status_rows.append(status_row)


def get_nr_of_combinations(row: StatusRow, combinations: list[str] = []) -> list[str]:
    global combinations_tried
    global combinations_found
    combinations_tried += 1
    broken_pattern = row.Broken_Pattern
    broken_groups = row.find_broken_groups()
    broken_groups_start_indexes = row.find_broken_groups_start_indexes()
    if len(broken_groups) == len(broken_pattern):
        # if it contains same numbers
        if all(
            [broken_groups[i] == broken_pattern[i] for i in range(len(broken_groups))]
        ):
            # row.print()
            combinations_found += 1
            combinations.append(str(row))

    # if pattern is longer than broken groups
    if len(broken_groups) < len(broken_pattern):
        free_spots = row.list_free_spots()
        for spot in free_spots:
            clone = row.clone()
            clone.Statuses[spot] = Status.Broken
            get_nr_of_combinations(clone, combinations)
    elif len(broken_groups) > len(broken_pattern):
        return combinations
    else:  # if pattern and broken groups are same length
        for i in range(len(broken_groups)):
            current_size = broken_groups[i]
            actual_size = broken_pattern[i]
            starts_at = broken_groups_start_indexes[i]
            next_unknown = row.get_next_unknown(starts_at)
            previous_unknown = row.get_previous_unknown(starts_at)
            if current_size == actual_size:
                continue
            elif current_size < actual_size:
                if next_unknown != -1:
                    clone = row.clone()
                    clone.Statuses[next_unknown] = Status.Broken
                    get_nr_of_combinations(clone, combinations)
                if previous_unknown != -1:
                    clone = row.clone()
                    clone.Statuses[previous_unknown] = Status.Broken
                    get_nr_of_combinations(clone, combinations)
            elif current_size > actual_size:
                return combinations
    return combinations


sum = 0
combinations_tried = 0
combinations_found = 0
rows_solved = 0


def print_status():
    global combinations_tried
    global combinations_found
    global rows_solved

    os.system("cls")
    print(f"Combinations tried: {combinations_tried}")
    print(f"Combinations found: {combinations_found}")
    print(f"Rows solved: {rows_solved}")
    print(f"Total Rows: {len(status_rows)}")
    print(f"Progress: {round(rows_solved / len(status_rows) * 100, 2)}%")
    print(f"Sum: {sum}")


async def process_row(row: StatusRow):
    global rows_solved
    combs = get_nr_of_combinations(row)
    rows_solved += 1
    print(f"Found {len(combs)} combinations for row {row}")
    nr_of_combinations = len(set(combs))
    print(f"Found {nr_of_combinations} Unique")
    return nr_of_combinations


async def process():
    global sum
    tasks = []
    for row in status_rows:
        tasks.append(asyncio.create_task(process_row(row)))
    results = await asyncio.gather(*tasks)
    sum = sum(results)
    print(f"Sum: {sum}")


async def main():
    loop = asyncio.get_running_loop()
    loop.set_default_executor(
        ThreadPoolExecutor(max_workers=40, thread_name_prefix="day-9")
    )
    await process()


if __name__ == "__main__":
    asyncio.run(main())
