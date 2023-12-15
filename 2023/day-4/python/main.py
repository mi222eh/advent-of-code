from pathlib import Path
from typing import Dict, List

result1 = 0


def to_int(input: str) -> int:
    return int(input.strip())


class Card:
    id: int
    my_numbers: list[int] = []
    drawn_numbers: list[int] = []

    @staticmethod
    def new_from_str(card_str: str):
        new_card = Card()
        id_str, card_content = card_str.split(":")
        new_card.id = int(id_str.split().pop())

        number_str, drawn_number_str = card_content.split("|")
        new_card.drawn_numbers = list(map(to_int, drawn_number_str.split()))
        new_card.my_numbers = list(map(to_int, number_str.split()))

        return new_card

    def get_matches(self) -> list[int]:
        won_numbers = filter(
            lambda nr: any(nr == drawn for drawn in self.drawn_numbers),
            self.my_numbers,
        )
        return won_numbers

    def get_nr_of_matches(self) -> int:
        return len(list(self.get_matches()))

    def copy(self):
        new_card = Card()
        new_card.my_numbers = self.my_numbers
        new_card.drawn_numbers = self.drawn_numbers
        new_card.id = self.id
        return new_card

    def get_points(self) -> int:
        points = 0
        won_numbers = self.get_matches()
        for n in range(0, len(list(won_numbers))):
            if points == 0:
                points = 1
            else:
                points = points * 2
        return points


class CardDeck:
    card_deck: Dict[int, List[Card]] = {}

    def get_cards(self, id: int) -> List[Card]:
        card_list = self.card_deck.get(id)
        if not card_list:
            self.card_deck[id] = []
        return self.card_deck[id]

    def add_card(self, card: Card):
        list_of_cards = self.get_cards(card.id)
        list_of_cards.append(card)

    def get_highest_id(self):
        return max(self.card_deck.keys())

    def get_nr_of_cards(self):
        total = 0
        for key in self.card_deck:
            total += len(self.card_deck.get(key))
        return total


card_deck = CardDeck()

content = Path("input.txt").read_text()
for card in map(Card.new_from_str, content.split("\n")):
    card_deck.add_card(card)


for id in range(0, card_deck.get_highest_id()):
    cards = card_deck.get_cards(id)
    for card in cards:
        nr_of_matches = card.get_nr_of_matches()
        # print(f"Card {card.id} has {nr_of_matches} matches")
        for n in range(1, nr_of_matches + 1):
            copied = card_deck.get_cards(card.id + n + 1).pop().copy()
            print(f"Copying card {copied.id}")
            card_deck.add_card(copied)


print(card_deck.get_nr_of_cards())
