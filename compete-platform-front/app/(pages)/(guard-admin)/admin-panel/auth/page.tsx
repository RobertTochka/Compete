export default function AdminPanelAuthPage() {
  return (
    <div className="w-full min-h-screen bg-gradient-to-tl from-GRADIK-2 to-GRADIK-1 p-10 flex-middle">
      <div className="relative max-w-[320px] w-full">
        <div className="w-full p-6 m-auto bg-saturateBlue/25 border-t-4 border-saturateBlue rounded-md shadow-md border-top">
          <h1 className="text-3xl font-semibold text-center">
            <img className="w-[90%] block mx-auto py-2.5" src="/img/logo.png" />
          </h1>
          <form className="mt-1.5">
            <div>
              <label
                htmlFor="email"
                className="block text-sm text-white/30 ml-1.5"
              >
                Логин
              </label>
              <input className="block w-full px-4 py-2 mt-1 text-white font-medium bg-saturateBlue/20 rounded-md focus:border-saturateBlue focus:ring-saturateBlue/60 focus:outline-none focus:ring focus:ring-opacity-30" />
            </div>
            <div className="mt-2">
              <div>
                <label
                  htmlFor="password"
                  className="block text-sm text-white/30 ml-1.5"
                >
                  Пароль
                </label>
                <input
                  type="password"
                  className="block w-full px-4 py-2 mt-1 text-white font-medium bg-saturateBlue/20 rounded-md focus:border-saturateBlue focus:ring-saturateBlue/60 focus:outline-none focus:ring focus:ring-opacity-30"
                />
              </div>
              <div className="mt-8">
                <button className="w-full px-4 py-2.5 tracking-wide text-white transition-colors transform bg-saturateBlue rounded-md hover:bg-secondaryBlue">
                  Войти
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
