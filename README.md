# HydroNotifier2

The project was realized for a customer who has a small water plant on the confluence of two rivers: Olse and Lomna.

It does:

    scrape public data about river levels from http://hydro.chmi.cz
    evaluates the sum of 2 river levels - low, medium, or high based on predefined values
    send SMS and email notifications to the customer in case of water level changes significantly

Integrations: Nexmo: SMS and email notifications Web scraping: http://hydro.chmi.cz

Deployed as time-triggered Azure function.
