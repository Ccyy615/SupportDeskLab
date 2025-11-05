using System;
using System.Collections.Generic;
using static SupportDeskLab.Utility;


namespace SupportDeskLab
{
   
     
    class Program
    {
        static int NextTicketId = 1;

        //Create Customer Dictionary
        static Dictionary<string, Customer> Customers = new Dictionary<string, Customer>();
        //create Ticket Queue
        static Queue<Ticket> Tickets = new Queue<Ticket>();
        //Create UndoEvent stack
        static Stack<UndoEvent> UndoStack = new Stack<UndoEvent>();

        static void Main()
        {
            initCustomer();

            while (true)
            {
                Console.WriteLine("\n=== Support Desk ===");
                Console.WriteLine("[1] Add customer");
                Console.WriteLine("[2] Find customer");
                Console.WriteLine("[3] Create ticket");
                Console.WriteLine("[4] Serve next ticket");
                Console.WriteLine("[5] List customers");
                Console.WriteLine("[6] List tickets");
                Console.WriteLine("[7] Undo last action");
                Console.WriteLine("[0] Exit");
                Console.Write("Choose: ");
                string choice = Console.ReadLine();

                //create switch cases and then call a reletive method 
                //for example for case 1 you need to have a method named addCustomer(); or case 2 add a method name findCustomer

                switch (choice)
                {
                    case "1": AddCustomer(); break;
                    case "2": FindCustomer(); break;
                    case "3": CreateTicket(); break;
                    case "4": ServeNext(); break;
                    case "5": ListCustomers(); break;
                    case "6": ListTickets(); break;
                    case "7": Undo(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }
        /*
         * Do not touch initCustomer method. this is like a seed to have default customers.
         */
        static void initCustomer()
        {
            //uncomments these 3 lines after you create the Customer Dictionary
            Customers["C001"] = new Customer("C001", "Ava Martin", "ava@example.com");
            Customers["C002"] = new Customer("C002", "Ben Parker", "ben@example.com");
            Customers["C003"] = new Customer("C003", "Chloe Diaz", "chloe@example.com");
        }

        static void AddCustomer()
        {
            //look at the Demo captuerd image and add your code here
            Console.WriteLine("-- Add Customer --");
            Console.WriteLine("Enter customer ID: ");
            String id = Console.ReadLine();
            Console.WriteLine("Enter customer Name: ");
            String name = Console.ReadLine();
            Console.WriteLine("Enter customer Email: ");
            String email = Console.ReadLine();
            Customers[id] = new Customer(id, name, email);
            Customer newCustomer = Customers[id];
            Console.WriteLine("Customer added: " + newCustomer);
            Console.WriteLine("");

            UndoStack.Push(new UndoAddCustomer(newCustomer));

        }

        static void FindCustomer()
        {
            //look at the Demo captuerd image and add your code here
            Console.WriteLine("-- Find Customer --");
            Console.WriteLine("Enter customer ID: ");
            String id = Console.ReadLine();
            if (Customers.ContainsKey(id))
            {
                Console.WriteLine("Customer found: " + Customers[id]);
            }
            else
            {
                Console.WriteLine("Customer not found.");
            }
            Console.WriteLine();

        }

        static void CreateTicket()
        {
            //look at the Demo captuerd image and add your code here
            Console.WriteLine("-- Create Ticket --");
            Console.WriteLine("Enter customer ID: ");
            String id = Console.ReadLine();
            Console.WriteLine("Enter ticket subject: ");
            String subject = Console.ReadLine();

            Ticket newTicket = new Ticket(NextTicketId++, id, subject);
            Tickets.Enqueue(newTicket);
            Console.WriteLine("Ticket created: " + newTicket);

            UndoStack.Push(new UndoCreateTicket(newTicket));   

        }

        static void ServeNext()
        {
            //look at the Demo captuerd image and add your code here
            Console.WriteLine("-- Serve Next Ticket --");
            if (Tickets.Count == 0)
            {
                Console.WriteLine("No tickets to serve.");
                return;
            }
            if (Tickets.Count > 0)
            {
                Ticket servedTicket = Tickets.Dequeue();
                Console.WriteLine("Served ticket: " + servedTicket);
            }
            
            UndoStack.Push(new UndoServeTicket(null));
        }

        static void ListCustomers()
        {
            Console.WriteLine("-- Customers --");
            //look at the Demo captuerd image and add your code here
            
            foreach (var customer in Customers.Values)
            {
                Console.WriteLine(customer);
            }


        }

        static void ListTickets()
        {
           
            Console.WriteLine("-- Tickets (front to back) --");
            //look at the Demo captuerd image and add your code here
            
            foreach (var ticket in Tickets)
            {
                Console.WriteLine(ticket);
            }

        }

        static void Undo()
        {
            //look at the Demo captuerd image and add your code here
            Console.WriteLine("-- Undo Last Action --");

            if (UndoStack.Count == 0)
            {
                Console.WriteLine("Nothing to undo.");

            }
            UndoEvent lastAction = UndoStack.Pop();

            switch (lastAction)
            {
                case UndoAddCustomer deleteCustomer:
                    {
                        Customers.Remove(deleteCustomer.Customer.CustomerId);
                        Console.WriteLine("Undo add new customer");
                        break;
                    }
                case UndoCreateTicket deleteTicket:
                    {
                        Queue<Ticket> tempQ = new Queue<Ticket>();
                        while (Tickets.Count > 0)
                        {
                            Ticket ticket = Tickets.Dequeue();
                            if (ticket.TicketId != deleteTicket.Ticket.TicketId)
                            {
                                tempQ.Enqueue(ticket);
                            }
                        }
                        Tickets = tempQ; // Reassign the filtered queue back to Tickets
                        Console.WriteLine("Undo add new ticket");
                        break;
                    }
                case UndoServeTicket undoServe:
                    {
                        if (Tickets.Count == 0)
                        {
                            Tickets.Enqueue(undoServe.Ticket);
                        }
                        else
                        {
                            Queue<Ticket> tempQueue = new Queue<Ticket>();
                            tempQueue.Enqueue(undoServe.Ticket);
                            while (Tickets.Count > 0)
                            {
                                tempQueue.Enqueue(Tickets.Dequeue());
                            }
                            Tickets = tempQueue;
                        }
                        Console.WriteLine("Undo: Re-add served ticket");
                        break;
                    }
            }



        }
    }
}

