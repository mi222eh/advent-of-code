// For more information see https://aka.ms/fsharp-console-apps
open System
open System.Linq
open System.IO

let GetValueFromCard (card) =

    match card with
    | 'T' -> 10
    | 'J' -> 1
    | 'Q' -> 12
    | 'K' -> 13
    | 'A' -> 14
    | _ ->  if Char.IsDigit(card) then
                int (string card)
            else failwith ("Unknown Card")

type Hand(cards, bid) =
    member val Cards: char array = cards with get, set
    member val Bid: int = bid with get, set

    member this.GetJokerCards() =
        this.Cards |> Array.filter (fun (c) -> c = 'J')

    member this.GetFiveOfAKindCards() =
        this.Cards
        |> Array.groupBy id
        |> Array.map (fun (card, cards) ->
            if card = 'J' then
                cards
            else
                cards.ToList().Concat(this.GetJokerCards().ToList()).ToArray())
        |> Array.tryFind (fun (cards) -> cards.Length = 5)

    member this.IsFiveOfAKind() =
        match this.GetFiveOfAKindCards() with
        | Some(_) -> true
        | None -> false

    member this.GetFourOfAKindCards() =
        this.Cards
        |> Array.groupBy id
        |> Array.map (fun (card, cards) ->
            if card = 'J' then
                cards
            else
                cards.ToList().Concat(this.GetJokerCards().ToList()).ToArray())
        |> Array.tryFind (fun (cards) -> cards.Length = 4)

    member this.IsFourOfAKind() =
        match this.GetFourOfAKindCards() with
        | Some(_) -> true
        | None -> false

    member this.IsFullHouse() =
        let jokerCards = this.GetJokerCards()
        let threeOfAKindCards = this.GetThreeOfAKindCards()

        match threeOfAKindCards with
        | Some(cards) ->
            let nrOfJokersInCards = cards |> Array.filter (fun c -> c = 'J') |> Array.length
            let remainingJokerCards = jokerCards |> Array.take(jokerCards.Length - nrOfJokersInCards)

            let remainingCards =
                this.Cards
                |> Array.filter (fun c -> c <> cards.[0] && c <> cards.[1] && c <> cards.[2])

            let remainingHand =
                new Hand(remainingCards |> Array.append remainingJokerCards, this.Bid)

            remainingHand.IsOnePair()
        | None -> false

    member this.GetThreeOfAKindCards() =
        this.Cards
        |> Array.groupBy id
        |> Array.map (fun (card, cards) ->
            if card = 'J' then
                cards
            else
                cards.ToList().Concat(this.GetJokerCards().ToList()).ToArray())
        |> Array.tryFind (fun (cards) -> cards.Length = 3)

    member this.IsThreeOfAKind() =
        match this.GetThreeOfAKindCards() with
        | Some(_) -> true
        | None -> false

    member this.IsTwoPair() =
        let jokerCards = this.GetJokerCards()
        let onePairCards = this.GetOnePairCards()

        match onePairCards with
        | Some(cards) ->
            let nrOfJokersInCards = cards |> Array.filter (fun c -> c = 'J') |> Array.length
            let remainingJokerCards = jokerCards |> Array.take(jokerCards.Length - nrOfJokersInCards)
            let remainingCards = this.Cards |> Array.filter (fun c -> c <> cards[0] && c <> cards[1])

            let remainingHand =
                new Hand(remainingCards |> Array.append remainingJokerCards, this.Bid)

            remainingHand.IsOnePair()
        | None -> false

    member this.GetOnePairCards() =
        this.Cards
        |> Array.groupBy id
        |> Array.map (fun (card, cards) ->
            if card = 'J' then
                cards
            else
                cards.ToList().Concat(this.GetJokerCards().ToList()).ToArray())
        |> Array.tryFind (fun (cards) -> cards.Length = 2)

    member this.IsOnePair() = 
        match this.GetOnePairCards() with
        | Some(_) -> true
        | None -> false

    member this.IsHighCard() =
        this.Cards |> Array.groupBy id |> Array.length = 5

    member this.GetValueFromCardIndex(idx) = GetValueFromCard(this.Cards[idx])

    member this.GetHandValue() =
        if this.IsFiveOfAKind() then 7
        else if this.IsFourOfAKind() then 6
        else if this.IsFullHouse() then 5
        else if this.IsThreeOfAKind() then 4
        else if this.IsTwoPair() then 3
        else if this.IsOnePair() then 2
        else if this.IsHighCard() then 1
        else failwith ("Unknown Hand")

    static member FromInput(input: string) : Hand =
        let cards, bidStr =
            input.Split(" ")
            |> Array.toList
            |> function
                | cards :: bidStr :: _ -> cards, bidStr
                | cards :: _ -> failwith ("Cards is defined but not the bid")
                | _ -> failwith ("Invalid Input")

        new Hand(cards.ToCharArray(), int (bidStr))

let HandleHandSort (handA: Hand, handB: Hand) : int =
    let valueA = handA.GetHandValue()
    let valueB = handB.GetHandValue()

    if valueA > valueB then
        1
    else if valueA < valueB then
        -1
    else
        let rec loop n = 
            if n = 5 then 0 
            else
                let cardValueA = handA.GetValueFromCardIndex(n)
                let cardValueB = handB.GetValueFromCardIndex(n)

                if cardValueA > cardValueB then
                    1
                elif cardValueA < cardValueB then
                    -1
                else
                    loop(n + 1)
        loop(0)
let fileContent = File.ReadAllLines(Path.Combine("..", "..", "..", "input.txt"))

let resultList =
    fileContent
    |> Array.map Hand.FromInput
    |> Array.sortWith (fun (a) -> fun (b) -> HandleHandSort(a, b))
    |> Array.mapi (fun (i) -> fun (c) -> (c, i + 1))

resultList |> Array.iter (fun((hand, index)) ->  printfn $"{index}: {String.Concat(hand.Cards)} {hand.Bid} -- {hand.GetHandValue()}")

let result =
    resultList
    |> Array.sumBy (fun ((hand, index)) -> hand.Bid * (index))

printfn $"Result is {result}"

printfn "Press any key to continue..."
System.Console.ReadKey(true) |> ignore