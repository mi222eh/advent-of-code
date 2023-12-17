import { every, filter, groupBy, has, range, some, sum, floor } from "https://cdn.skypack.dev/lodash?dts";

function getValueFromCard(card:string){
    const potentialNumber = Number(card);

    if(!isNaN(potentialNumber))
        return potentialNumber
    
    switch (card) {
        case "T": return 10
        case "J": return 1
        case "Q": return 12
        case "K": return 13
        case "A": return 14
        default:throw Error("Unknown Card")
    }
}

function getHandValued(hand:Hand){
    if (hand.isFiveOfAKind()) return 7
    if (hand.isFourOfAKind()) return 6
    if (hand.isFullHouse()) return 5
    if (hand.isThreeOfAKind()) return 4
    if (hand.isTwoPair()) return 3
    if (hand.isOnePair()) return 2
    if (hand.isHighCard()) return 1
    throw Error("Unknown Hand")
}

class Hand{
    cards:string[] = []
    bid = 0 
    
    
    getGroupedCards = () => groupBy(this.cards)
    getGroupedCardsNoJokers = () => groupBy(this.cards.filter(c => c !== "J"))

    isFiveOfAKind = () =>  5 - this.getNrOfJokers() === 0 ? true : some(this.getGroupedCards(), (c) => c.length === (5 - this.getNrOfJokers())) 
    isFourOfAKind = () =>  4 - this.getNrOfJokers() === 0 ? true : some(this.getGroupedCards(), (c) => c.length === (4 - this.getNrOfJokers()))
    isFullHouse = () => {
        const groupedCards = this.getGroupedCardsNoJokers();
        let counts = Object.values(groupedCards).map(c => c.length);
        let nrOfJokers = this.getNrOfJokers();

        let hasThreeOfAKind = false;
        let toakidx = counts.indexOf(3)
        if (toakidx > -1){
            hasThreeOfAKind = true
            counts.splice(toakidx, 1)
        }

        toakidx = counts.indexOf(2)
        if (!hasThreeOfAKind && toakidx > -1 && nrOfJokers >= 1){
            hasThreeOfAKind = true
            nrOfJokers -= 1;
            counts.splice(toakidx, 1)
        }
        
        toakidx = counts.indexOf(1)
        if (!hasThreeOfAKind && toakidx > -1 && nrOfJokers >= 2){
            hasThreeOfAKind = true
            nrOfJokers -= 2;
            counts.splice(toakidx, 1)
        }

        let hasPair = false;
        let pIdx = counts.indexOf(2)

        if (pIdx > -1){
            hasPair = true;
        }

        pIdx = counts.indexOf(1)
        if (!hasPair && pIdx > -1 && nrOfJokers >= 1){
            hasPair = true;
            nrOfJokers -= 1;
        }


        return hasThreeOfAKind && hasPair
    }
    isThreeOfAKind = () =>  3 - this.getNrOfJokers() === 0 ? true : some(this.getGroupedCards(), (c) => c.length === (3 - this.getNrOfJokers())) 
    isTwoPair = () => filter(this.getGroupedCards(), (c) => c.length === 2).length === (2 - floor(this.getNrOfJokers() / 2))
    isOnePair = () => 2 - this.getNrOfJokers() === 0 ? true : some(this.getGroupedCards(), (c) => c.length === (2 - this.getNrOfJokers()))
    isHighCard = () => every(this.getGroupedCards(), (c) => c.length === 1)

    getNrOfJokers = () => this.cards.filter(c => c === 'J').length

    static FromInput(input:string): Hand{
        const [cards, bidStr] = input.split(" ")
        let hand = new Hand();
        hand.bid = Number(bidStr);
        hand.cards = cards.split("");
        return hand;
    }
}

let input = await Deno.readTextFile("input.txt")
let hands = input.split("\n").map(Hand.FromInput)


let sortedHands = hands.sort((handA, handB) => {
    let valueA = getHandValued(handA);
    let valueB = getHandValued(handB);
    if (valueA > valueB) return 1; 
    if (valueA < valueB) return -1;
    
    for (const n of range(0, 5)) {
        let cardValueA = getValueFromCard(handA.cards[n])
        let cardValueB = getValueFromCard(handB.cards[n])
        
        if (cardValueA > cardValueB) return 1; 
        if (cardValueA < cardValueB) return -1;
    }
    return 0;
})

sortedHands.forEach(c => console.log(c.cards))

let scores = sortedHands.map((hand, i) => hand.bid * (i + 1));

let result = sum(scores);

console.log(result);