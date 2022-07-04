
**Simple Exchange**



# Tasks
## 1. Implement a simplistic framework, containing above components with an object-oriented programming language of your choice. Please note that efficiency & performance of your code will be evaluated.



## 2. Write a script that generates dummy messages and sends these messages to your exchange. There is no need to use real-market data and therefore you can make an imaginary market.

Please note Test project under \bitso\test\Bitso.Challenge.Service.Test

## 3- Assess the performance of your code theoretically (for each distinct message received and for executions), by means of Big O notation. Comment on the bottlenecks on each cycle.

  Created benchmark test cases for 

### Update

-Update Order message performs a binary search O(log(n)) and then moves found node to back using a Linked List O(1) so order is o(log(n))


### Remove

-While processing the Remove Order message, the bottleneck comes mainly from deleting key from Hash Map which has O(n) complexity (might need to move n-1 elements from the array). 
  

### Add

-If having a full match might run up to k*O(n) due to removing opposite trades where k is amount of matched trades
  

## 4- Stress-test your framework with a heavily liquid order book. You may assume 100 price levels at both sides with 2000 orders at each level. Measure and compare processing time of different messages on some random orders.

  

* About 100 K orders inserted in 253ms

* About 100 K orders filled in 883 ms

* Deleted about 200 K orders deleted in 587 ms

* Changed Around 200K orders in 62 ms