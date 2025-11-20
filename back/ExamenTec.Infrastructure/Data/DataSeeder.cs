using ExamenTec.Application.Services;
using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;

namespace ExamenTec.Infrastructure.Data;

public class DataSeeder
{
    private readonly IUnitOfWork _unitOfWork;

    public DataSeeder(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task SeedCategoriesAsync()
    {
        var existingCategories = await _unitOfWork.Categories.GetAllAsync();
        if (existingCategories.Any())
        {
            return;
        }

        var categories = new List<(string Name, string? Description)>
        {
            ("Electrónica y Tecnología", "Dispositivos electrónicos, smartphones, tablets y gadgets tecnológicos"),
            ("Ropa y Accesorios", "Prendas para hombres, mujeres y niños"),
            ("Hogar y Jardín", "Artículos para el hogar, decoración y jardinería"),
            ("Deportes y Fitness", "Equipamiento deportivo y accesorios para ejercicio"),
            ("Libros y Medios", "Libros físicos y digitales, revistas y medios impresos"),
            ("Juguetes y Juegos", "Juguetes educativos y de entretenimiento para todas las edades"),
            ("Alimentos y Bebidas", "Productos alimenticios frescos, enlatados y bebidas"),
            ("Belleza y Cuidado Personal", "Cosméticos, productos de belleza y cuidado personal"),
            ("Salud y Bienestar", "Suplementos, vitaminas y productos para la salud"),
            ("Automotriz", "Repuestos, accesorios y productos para vehículos"),
            ("Mascotas", "Alimentos, accesorios y productos para mascotas"),
            ("Arte y Manualidades", "Materiales para arte, manualidades y proyectos creativos"),
            ("Instrumentos Musicales", "Instrumentos musicales y accesorios"),
            ("Herramientas y Construcción", "Herramientas manuales y eléctricas para construcción"),
            ("Viajes y Turismo", "Equipaje, accesorios de viaje y productos turísticos"),
            ("Oficina y Negocios", "Suministros de oficina y productos para negocios"),
            ("Fotografía", "Cámaras, lentes y accesorios fotográficos"),
            ("Computadoras y Accesorios", "Laptops, PCs, periféricos y accesorios informáticos"),
            ("Muebles", "Muebles para hogar y oficina"),
            ("Cocina y Utensilios", "Utensilios, electrodomésticos y accesorios de cocina"),
            ("Bebés y Niños", "Productos para bebés, ropa y accesorios infantiles"),
            ("Joyas y Relojes", "Joyas, relojes y accesorios de lujo"),
            ("Videojuegos", "Consolas, juegos y accesorios para videojuegos"),
            ("Películas y TV", "Películas, series y productos relacionados con entretenimiento"),
            ("Música", "CDs, discos de vinilo y productos musicales"),
            ("Jardinería", "Semillas, plantas, herramientas y productos para jardín"),
            ("Limpieza", "Productos de limpieza para hogar y oficina"),
            ("Iluminación", "Lámparas, bombillas y sistemas de iluminación"),
            ("Decoración", "Artículos decorativos para interiores y exteriores"),
            ("Seguridad del Hogar", "Sistemas de seguridad, alarmas y cerraduras"),
            ("Camping y Aire Libre", "Equipamiento de camping y actividades al aire libre"),
            ("Pesca y Caza", "Equipamiento y accesorios para pesca y caza"),
            ("Ciclismo", "Bicicletas, componentes y accesorios para ciclismo"),
            ("Running", "Calzado, ropa y accesorios para corredores"),
            ("Natación", "Trajes de baño, goggles y accesorios para natación"),
            ("Yoga y Pilates", "Accesorios y ropa para yoga y pilates"),
            ("Culturismo", "Pesas, máquinas y accesorios para gimnasio"),
            ("Suplementos Deportivos", "Proteínas, suplementos y productos deportivos"),
            ("Bicicletas", "Bicicletas completas y componentes"),
            ("Skateboarding", "Tablas, ruedas y accesorios para patineta"),
            ("Esquí y Nieve", "Equipamiento de esquí y deportes de invierno"),
            ("Surf", "Tablas de surf, trajes de neopreno y accesorios para surf"),
            ("Boxeo y Artes Marciales", "Equipamiento para boxeo y artes marciales"),
            ("Fútbol", "Balones, uniformes y accesorios para fútbol"),
            ("Básquetbol", "Balones, aros y accesorios para básquetbol"),
            ("Tenis", "Raquetas, pelotas y accesorios para tenis"),
            ("Golf", "Palos, pelotas y accesorios para golf"),
            ("Béisbol", "Bates, guantes y accesorios para béisbol"),
            ("Hockey", "Discos, palos y accesorios para hockey"),
            ("Voleibol", "Balones, redes y accesorios para voleibol"),
            ("Fitness y Cardio", "Máquinas cardiovasculares y equipamiento de fitness"),
            ("Drones y UAVs", "Drones comerciales, recreativos y profesionales con cámaras"),
            ("Realidad Virtual", "Gafas VR, controladores y experiencias de realidad virtual"),
            ("Smart Home", "Dispositivos inteligentes para automatización del hogar"),
            ("Energía Solar", "Paneles solares, baterías y sistemas de energía renovable"),
            ("Robótica", "Kits de robótica educativa y robots programables"),
            ("Impresión 3D", "Impresoras 3D, filamentos y accesorios de impresión"),
            ("Drones Racing", "Drones de carreras y accesorios para competencias"),
            ("Wearables", "Dispositivos portátiles inteligentes y trackers de actividad"),
            ("Audio Hi-Fi", "Equipos de audio de alta fidelidad y sistemas de sonido"),
            ("Vinilos y Discos", "Discos de vinilo, tocadiscos y accesorios para coleccionistas"),
            ("Streaming", "Equipos para streaming, cámaras y software de transmisión"),
            ("Criptomonedas", "Hardware wallets y accesorios para criptomonedas"),
            ("Blockchain", "Productos y servicios relacionados con tecnología blockchain"),
            ("Inteligencia Artificial", "Herramientas y software de IA para desarrollo"),
            ("Cloud Computing", "Servicios y productos de computación en la nube"),
            ("Ciberseguridad", "Software y hardware de seguridad informática"),
            ("Desarrollo Web", "Herramientas y recursos para desarrollo web"),
            ("Programación", "Libros, cursos y herramientas para programadores"),
            ("Redes y Conectividad", "Equipos de red, routers, switches y accesorios de conectividad"),
            ("Servidores y Data Centers", "Servidores, racks y equipamiento para centros de datos")
        };

        foreach (var (name, description) in categories)
        {
            var category = new Category
            {
                Name = name,
                Description = description,
                CreatedDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365))
            };

            await _unitOfWork.Categories.AddAsync(category);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SeedProductsAsync()
    {
        var existingProducts = await _unitOfWork.Products.GetAllAsync();
        if (existingProducts.Any())
        {
            return;
        }

        var categories = (await _unitOfWork.Categories.GetAllAsync()).ToList();

        if (!categories.Any())
        {
            return;
        }

        var products = new List<(string Name, string? Description, decimal Price, int Stock, int CategoryIndex, int StoreIndex)>
        {
            ("Smartphone Samsung Galaxy", "Smartphone con pantalla AMOLED de 6.7 pulgadas", 899.99m, 15, 0, 4),
            ("iPhone 15 Pro", "Último modelo de Apple con chip A17 Pro", 1299.99m, 10, 0, 4),
            ("Laptop HP Pavilion", "Laptop de 15.6 pulgadas, Intel Core i7, 16GB RAM", 899.99m, 8, 0, 4),
            ("Tablet iPad Air", "Tablet Apple con pantalla Retina de 10.9 pulgadas", 649.99m, 12, 0, 4),
            ("Audífonos Sony WH-1000XM5", "Audífonos con cancelación de ruido", 399.99m, 20, 0, 4),

            ("Playera de Algodón Básica", "Playera 100% algodón, múltiples colores disponibles", 19.99m, 50, 1, 5),
            ("Jeans Clásicos", "Jeans corte recto, tallas disponibles", 59.99m, 30, 1, 5),
            ("Tenis Deportivos", "Tenis cómodos para uso diario", 79.99m, 25, 1, 5),
            ("Chamarra de Cuero Genuino", "Chamarra de cuero genuino, tallas disponibles", 199.99m, 15, 1, 5),
            ("Reloj Inteligente", "Smartwatch con monitor de actividad", 249.99m, 18, 1, 5),

            ("Sofá de 3 Plazas", "Sofá cómodo con cojines removibles", 599.99m, 5, 2, 7),
            ("Mesa Central", "Mesa central de madera maciza", 149.99m, 10, 2, 7),
            ("Lámpara de Pie", "Lámpara LED con regulador de intensidad", 89.99m, 20, 2, 7),
            ("Cortinas Blackout", "Cortinas bloqueadoras de luz, varios colores", 69.99m, 30, 2, 7),
            ("Juego de Sábanas", "Sábanas de algodón egipcio, tamaño queen", 49.99m, 40, 2, 7),

            ("Balón de Fútbol", "Balón oficial talla 5", 29.99m, 35, 3, 6),
            ("Raqueta de Tenis", "Raqueta profesional con cuerdas incluidas", 149.99m, 12, 3, 6),
            ("Mancuernas Ajustables", "Par de mancuernas de 10kg", 79.99m, 20, 3, 6),
            ("Bicicleta de Montaña", "Bicicleta de montaña de 21 velocidades", 549.99m, 8, 3, 6),
            ("Mat para Yoga", "Mat antideslizante para yoga", 24.99m, 45, 3, 6),

            ("El Principito", "Clásico de Antoine de Saint-Exupéry", 14.99m, 60, 4, 8),
            ("Harry Potter - Edición Completa", "Colección completa de 7 libros", 89.99m, 15, 4, 8),
            ("Kindle Paperwhite", "Lector electrónico con luz integrada", 139.99m, 25, 4, 8),
            ("Audiolibro: La Odisea", "Versión narrada del clásico griego", 19.99m, 30, 4, 8),
            ("Revista National Geographic", "Suscripción anual - 12 números", 49.99m, 100, 4, 8),

            ("Lego Star Wars", "Set de construcción de 1000 piezas", 79.99m, 20, 5, 9),
            ("Monopolio Clásico", "Juego de mesa familiar", 34.99m, 30, 5, 9),
            ("Muñeca Barbie", "Muñeca Barbie con accesorios", 29.99m, 40, 5, 9),
            ("Hot Wheels - Paquete de 5", "Paquete con 5 autos coleccionables", 19.99m, 50, 5, 9),
            ("Set de Construcción Magnético", "Bloques magnéticos educativos", 49.99m, 25, 5, 9),

            ("Aceite de Oliva Extra Virgen", "Aceite de oliva 1 litro, primera prensa", 12.99m, 80, 6, 10),
            ("Arroz Integral 2kg", "Arroz integral de grano largo", 8.99m, 100, 6, 10),
            ("Café en Grano Premium", "Café 100% arábiga, paquete de 500g", 15.99m, 60, 6, 10),
            ("Miel Natural", "Miel pura sin procesar, frasco de 500g", 11.99m, 45, 6, 10),
            ("Galletas Integrales", "Galletas de avena y miel, paquete familiar", 4.99m, 120, 6, 10),

            ("Crema Facial Hidratante", "Crema con ácido hialurónico, 50ml", 24.99m, 55, 7, 11),
            ("Shampoo Reparador", "Shampoo para cabello dañado, 400ml", 12.99m, 70, 7, 11),
            ("Perfume Clásico", "Fragancia de larga duración, presentación 100ml", 59.99m, 30, 7, 11),
            ("Kit de Maquillaje", "Kit completo con 20 colores", 34.99m, 40, 7, 11),
            ("Cepillo de Dientes Eléctrico", "Cepillo recargable con temporizador", 49.99m, 35, 7, 11),

            ("Multivitamínico Completo", "Suplemento con 13 vitaminas esenciales", 19.99m, 90, 8, 12),
            ("Omega 3", "Cápsulas de aceite de pescado, frasco de 60 unidades", 24.99m, 65, 8, 12),
            ("Proteína en Polvo", "Proteína de suero, sabor vainilla, 1kg", 39.99m, 50, 8, 12),
            ("Termómetro Digital", "Termómetro infrarrojo sin contacto", 29.99m, 40, 8, 12),
            ("Báscula Digital Inteligente", "Báscula con conexión Bluetooth", 49.99m, 25, 8, 12),

            ("Aceite Motor 5W-30", "Aceite sintético 4 litros", 34.99m, 45, 9, 13),
            ("Llanta 205/55R16", "Llanta todo terreno", 89.99m, 30, 9, 13),
            ("Batería de Auto 12V", "Batería de 60Ah para vehículos", 149.99m, 15, 9, 13),
            ("Filtro de Aire", "Filtro de aire premium", 19.99m, 50, 9, 13),
            ("Kit de Limpieza para Auto", "Kit completo con productos especializados", 29.99m, 35, 9, 13),

            ("Alimento Premium para Perro", "Alimento seco para adultos, bolsa de 15kg", 49.99m, 40, 10, 14),
            ("Alimento para Gato", "Alimento húmedo en latas, paquete de 12", 18.99m, 55, 10, 14),
            ("Juguete para Perro", "Pelota resistente con cuerda", 9.99m, 60, 10, 14),
            ("Cama para Mascota", "Cama ortopédica tamaño grande", 39.99m, 20, 10, 14),
            ("Correa Retráctil", "Correa extensible hasta 5 metros", 19.99m, 45, 10, 14),

            ("Acuarelas Profesionales", "Set de 24 colores con pinceles", 29.99m, 30, 11, 15),
            ("Block de Dibujo A4", "Block con 50 hojas de papel grueso", 12.99m, 50, 11, 15),
            ("Tijeras para Diseño", "Tijeras profesionales para papel", 14.99m, 40, 11, 15),
            ("Set de Pinceles", "Set completo con 10 pinceles diferentes", 19.99m, 35, 11, 15),
            ("Lápices de Colores", "Set de 36 lápices profesionales", 24.99m, 40, 11, 15),

            ("Guitarra Acústica", "Guitarra de cuerdas de acero, tamaño completo", 299.99m, 10, 12, 16),
            ("Piano Digital", "Piano de 88 teclas con sonidos de alta calidad", 599.99m, 5, 12, 16),
            ("Batería Electrónica", "Batería electrónica con pads", 449.99m, 6, 12, 16),
            ("Micrófono USB", "Micrófono condensador para grabación", 89.99m, 20, 12, 16),
            ("Afinador de Guitarra", "Afinador cromático digital", 19.99m, 45, 12, 16),

            ("Taladro Eléctrico", "Taladro inalámbrico 20V con 2 baterías", 149.99m, 15, 13, 17),
            ("Set de Destornilladores", "Set profesional con 20 piezas", 29.99m, 40, 13, 17),
            ("Martillo de Carpintero", "Martillo con mango de fibra de vidrio", 19.99m, 50, 13, 17),
            ("Multímetro Digital", "Medidor digital para electricidad", 49.99m, 25, 13, 17),
            ("Nivel Láser", "Nivel láser con líneas horizontales y verticales", 79.99m, 20, 13, 17),

            ("Maleta con Ruedas", "Maleta de viaje 70cm con sistema 360°", 129.99m, 25, 14, 18),
            ("Mochila de Viaje", "Mochila de 40 litros, resistente al agua", 59.99m, 30, 14, 18),
            ("Adaptador Universal", "Adaptador para viajes internacionales", 14.99m, 60, 14, 18),
            ("Almohada de Viaje", "Almohada cervical ergonómica", 19.99m, 45, 14, 18),
            ("Organizador de Viaje", "Kit organizador con bolsas compresoras", 24.99m, 35, 14, 18),

            ("Impresora Multifuncional", "Impresora láser con escáner y copiadora", 199.99m, 12, 15, 19),
            ("Escritorio de Oficina", "Escritorio de madera con cajones", 299.99m, 8, 15, 19),
            ("Silla Ergonómica", "Silla de oficina con soporte lumbar", 249.99m, 15, 15, 19),
            ("Organizador de Escritorio", "Organizador con múltiples compartimentos", 34.99m, 40, 15, 19),
            ("Calculadora Científica", "Calculadora con funciones avanzadas", 24.99m, 50, 15, 19)
        };

        foreach (var (name, description, price, stock, categoryIndex, storeIndex) in products)
        {
            if (categoryIndex < categories.Count)
            {
                var product = new Product
                {
                    Name = name,
                    Description = description,
                    Price = price,
                    Stock = stock,
                    CategoryId = categories[categoryIndex].Id,
                    CreatedDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365))
                };

                await _unitOfWork.Products.AddAsync(product);
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SeedUsersAsync()
    {
        var existingUsers = await _unitOfWork.Users.GetAllAsync();
        if (existingUsers.Any())
        {
            return;
        }

        var users = new List<(string Email, string Password, string Role)>
        {
            ("admin@examentec.com", "Admin123!", "Admin"),
            ("product@examentec.com", "Product123!", "Product"),
            ("category@examentec.com", "Category123!", "Category")
        };

        foreach (var (email, password, role) in users)
        {
            var user = new User
            {
                Email = email,
                PasswordHash = UserService.HashPassword(password),
                Role = role,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
        }
        await _unitOfWork.SaveChangesAsync();
    }
}
