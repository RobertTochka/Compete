"use client";
import { useKeenSlider } from "keen-slider/react";
import { useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";
import { AnimatePresence, motion } from "framer-motion";
import { useRouter } from "next/navigation";
import { useCheckAuthMutation } from "@/app/_fetures/lib/api/publicLobbiesApi";
import { getUserId } from "@/app/_utils/functions";

const StatItem = ({
  value,
  label,
}: {
  value: string | number;
  label: string;
}) => {
  return (
    <div className="flex flex-col gap-[5px] items-center">
      <span className="text-[64px] font-semibold">{value}</span>
      <span className="text-[20px] font-light">{label}</span>
    </div>
  );
};

const Arrow = ({ onClick, isLeft }: { onClick: any; isLeft?: boolean }) => {
  return (
    <svg
      onClick={onClick}
      className={`arrow ${isLeft ? "arrow--left" : "arrow--right"}`}
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 24 24"
    >
      {isLeft && (
        <path d="M16.67 0l2.83 2.829-9.339 9.175 9.339 9.167-2.83 2.829-12.17-11.996z" />
      )}
      {!isLeft && <path d="M5 3l3.057-3 11.943 12-11.943 12-3.057-3 9-9z" />}
    </svg>
  );
};

export default function Slider() {
  const searchParams = useSearchParams();
  const router = useRouter();

  const [currentSlide, setCurrentSlide] = useState(0);

  const [checkAuth, { isError }] = useCheckAuthMutation();
  const onPlay = () => {
    router.push("api/auth/enter");
  };
  useEffect(() => {
    const timerId = setTimeout(() => {
      checkAuth()
        .unwrap()
        .then(() => router.push(`/profile/${getUserId()}`));
    }, 1000);
    return () => {
      clearInterval(timerId);
    };
  }, []);
  const [loaded, setLoaded] = useState(false);
  const [sliderRef, instanceRef] = useKeenSlider<HTMLDivElement>({
    loop: true,
    initial: 0,
    slideChanged(slider) {
      setCurrentSlide(slider.track.details.rel);
    },
    created() {
      setLoaded(true);
    },
  });

  useEffect(() => {
    const slideTag = decodeURIComponent(
      window.location.href.split("#").at(-1) || ""
    );

    const slidesIdx: any = {
      Матчмейкинг: 1,
      Рейтинг: 2,
      "Техническая поддержка": 3,
    };

    if (slidesIdx[slideTag]) {
      instanceRef.current?.moveToIdx(slidesIdx[slideTag]);
    } else {
      instanceRef.current?.moveToIdx(0);
    }
  }, [searchParams]);

  return (
    <div className="mt-[90px] relative">
      <section
        ref={sliderRef}
        className="keen-slider flex items-center max-h-[633px] h-full"
      >
        <div className="keen-slider__slide">
          <div className="max-w-[745px]">
            <h1 className="text-[64px] font-extrabold mb-12">
              Платформа соревновательных игр CS2
            </h1>
            <div className="flex items-start justify-between">
              <StatItem value={"1284"} label="Пользователей" />
              <StatItem value={"10027"} label="Матчей" />
              <StatItem value={"10 млн ₽"} label="Месячный оборот игроков" />
            </div>
            <p className="mt-14 mb-12 font-light text-[24px]">
              Организовывайте матчи и выигрывайте общий денежный банк,
              составленный двумя конкурирующими сторонами
            </p>
            <button
              onClick={onPlay}
              className="pt-[15px] pb-4 px-12 bg-blue rounded-[20px] font-semibold text-[40px] hover:bg-secondaryBlue"
            >
              Начать играть
            </button>
          </div>
        </div>
        <div className="keen-slider__slide">
          <div className="max-w-[486px]">
            <h1 className="text-[64px] font-extrabold">Матчмейкинг</h1>
            <p className="text-[36px] mt-[58px] xl:text-[32px]">
              Монетизируйте
              <br /> каждый матч
            </p>
          </div>
        </div>
        <div className="keen-slider__slide">
          <div className="max-w-[536px]">
            <h1 className="text-[96px] font-semibold">Рейтинг</h1>
            <p className="text-[36px] mt-7 xl:text-[28px] xl:max-w-[420px]">
              Система, позволяющая отслеживать место в топе по определенным
              параметрам: по количеству compete-поинтов, матчей и призовых.
            </p>
          </div>
        </div>
        <div className="keen-slider__slide">
          <div className="max-w-[850px]">
            <h1 className="text-[96px] font-semibold uppercase">
              Центр поддержки
            </h1>
            <p className="mt-10 text-[36px] text-justify  xl:text-[28px] xl:max-w-[700px]">
              Круглосуточная поддержка пользователей в{" "}
              <span className="font-semibold">telegram</span>, которая ответит
              на все ваши вопросы, заблокирует читеров и разрешит любые
              проблемы.
            </p>
          </div>
        </div>
      </section>
      <AnimatePresence>
        {currentSlide === 0 && (
          <motion.div
            className="w-[3000px] absolute -top-[32%] -right-[71%] -z-[1] xl:w-[1500px] xl:-top-[20%] xl:-right-[10%]"
            animate={{ opacity: 1 }}
            initial={{ opacity: 0 }}
            exit={{ opacity: 0 }}
          >
            <img src="/img/homepage/keyboard.png" />
            <div className="bg-blue w-[420px] h-[900px] absolute top-42 left-0 right-40 bottom-0 homepage-blue-shadow--keyboard rotate-[-51deg] m-auto rounded-xl -z-[1] xl:w-[280px] xl:h-[550px] xl:top-10 xl:-right-[40%]" />
          </motion.div>
        )}
      </AnimatePresence>
      <AnimatePresence>
        {currentSlide === 1 && (
          <motion.div
            className="absolute right-0 top-0 flex items-center -z-[1] gap-[75px] xl:top-1/2 xl:-translate-y-[65%]  xl:gap-5"
            animate={{ opacity: 1 }}
            initial={{ opacity: 0 }}
            exit={{ opacity: 0 }}
          >
            <div className="relative">
              <img
                className="xl:w-[350px]"
                src="/img/homepage/revenue-left.png"
              />
              <div className="bg-blue w-[95%] h-[95%] absolute top-0 left-0 right-0 bottom-0 m-auto homepage-blue-shadow--revenue -z-[1]" />
            </div>
            <div className="relative">
              <img
                className="xl:w-[350px]"
                src="/img/homepage/revenue-right.png"
              />
              <div className="bg-blue w-[95%] h-[95%] absolute top-0 left-0 right-0 bottom-0 m-auto homepage-blue-shadow--revenue -z-[1]" />
            </div>
          </motion.div>
        )}
      </AnimatePresence>
      <AnimatePresence>
        {currentSlide === 2 && (
          <motion.div
            className="absolute right-0 top-0 flex items-center -z-[1] gap-[75px] xl:gap-5 xl:top-1/2 xl:-translate-y-[65%]"
            animate={{ opacity: 1 }}
            initial={{ opacity: 0 }}
            exit={{ opacity: 0 }}
          >
            <div className="relative">
              <img
                className="xl:w-[350px]"
                src="/img/homepage/rating-left.png"
              />
              <div className="bg-blue w-[95%] h-[95%] absolute top-0 left-0 right-0 bottom-0 m-auto homepage-blue-shadow--rating -z-[1]" />
            </div>
            <div className="relative">
              <img
                className="xl:w-[350px]"
                src="/img/homepage/rating-right.png"
              />
              <div className="bg-blue w-[95%] h-[95%] absolute top-0 left-0 right-0 bottom-0 m-auto homepage-blue-shadow--rating -z-[1]" />
            </div>
          </motion.div>
        )}
      </AnimatePresence>
      <AnimatePresence>
        {currentSlide === 3 && (
          <motion.div
            className="absolute -z-[1] -top-4 right-[145px] xl:right-20"
            animate={{ opacity: 1 }}
            initial={{ opacity: 0 }}
            exit={{ opacity: 0 }}
          >
            <img className="xl:w-[360px]" src="/img/homepage/chat.png" />
            <div className="bg-blue w-[90%] h-[90%] absolute top-0 left-0 right-0 bottom-0 m-auto homepage-blue-shadow--chat -z-[1]" />
          </motion.div>
        )}
      </AnimatePresence>
      {loaded && instanceRef.current && (
        <>
          <Arrow
            isLeft
            onClick={(e: any) =>
              e.stopPropagation() || instanceRef.current?.prev()
            }
          />

          <Arrow
            onClick={(e: any) =>
              e.stopPropagation() || instanceRef.current?.next()
            }
          />
        </>
      )}
      {loaded && instanceRef.current && (
        <div className="flex gap-[18px] mt-[100px] ml-[300px]">
          {[
            ...Array(instanceRef.current.track.details.slides.length).keys(),
          ].map((idx) => {
            return (
              <button
                key={idx}
                onClick={() => {
                  instanceRef.current?.moveToIdx(idx);
                }}
                className={`w-3 h-3 rounded-full blur-[2px] ${
                  currentSlide === idx ? "bg-[#DEB8FF]" : "bg-[#3E3E64]"
                }`}
              ></button>
            );
          })}
        </div>
      )}
    </div>
  );
}
