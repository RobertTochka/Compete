import { useSearchParams } from "next/navigation";
import React, { useEffect, useState } from "react";
import { PaymentMethod } from "../PaymentMethod/PaymentMethod";
import {
  useDoPayoutMutation,
  useDoPayMutation,
} from "@/app/_fetures/lib/api/publicLobbiesApi";
import { handleError } from "@/app/_utils/functions";
import Icon from "../Icon";
import { PayoutModal } from "../PayoutModal/PayoutModal";
import { PayModal } from "../PayModal/PayModal";
import { ResultModal } from "../ResultModal/ResultModal";
import { ConfirmationType } from "@/app/_utils/types";
import * as Yup from "yup";
type PayVariant = "bank-card" | "ymoney" | "sberpay" | "sbp" | "balance-phone";
const validationSchema = Yup.object().shape({
  amount: Yup.number()
    .min(5, "Минимум 5 рублей")
    .required("Значение является обязательным"),
});
const phoneValidationScheme = Yup.object().shape({
  phone: Yup.string()
    .matches(
      /^7\d{10}$/,
      "Номер телефона должен быть в формате 7XXXXXXXXXX и содержать 11 цифр"
    )
    .required("Номер телефона является обязательным"),
});
const PAYMENT_METHODS: { text: string; icon: PayVariant }[] = [
  {
    text: "Банковская карта",
    icon: "bank-card",
  },
  {
    text: "ЮMoney",
    icon: "ymoney",
  },
  {
    text: "SberPay",
    icon: "sberpay",
  },
  {
    text: "СБП",
    icon: "sbp",
  },
  {
    text: "Баланс телефона",
    icon: "balance-phone",
  },
];
export const BalanceWallet = () => {
  const [variant, setVariant] = useState<PayVariant>(PAYMENT_METHODS[0].icon);
  const [showPayModal, setShowPayModal] = useState(false);
  const [showPayModalLikePayout, setShowPayModalLikePayout] = useState(false);
  const [showPayoutModal, setShowPayoutModal] = useState(false);
  const searchParams = useSearchParams();
  const [resultIcon, setResultIcon] = useState<string | undefined>(undefined);
  const [cardIdentifier, setCardIdentifier] = useState<string>("");
  const [doPayout, { error: payoutError, isLoading: payoutLoading }] =
    useDoPayoutMutation();
  const [doPay, { isLoading: payLoading, error: payError }] =
    useDoPayMutation();
  const [modalError, setModalError] = useState<undefined | string>(undefined);
  const [sucessMessage, setSuccessMessage] = useState<undefined | string>(
    undefined
  );
  const [showPhoneGrub, setShowPhoneGrub] = useState(false);
  const [phone, setPhone] = useState("");
  useEffect(() => {
    const fromShop = searchParams.get("fromShop");
    if (typeof fromShop === "string") setSuccessMessage(fromShop);
  }, [searchParams]);
  useEffect(() => {
    if (payoutError) setModalError(handleError(payoutError));
    if (payError) setModalError(handleError(payError));
  }, [payoutError, payError]);
  const onCardIdentifierGrub = (cardId: string) => {
    setShowPayModalLikePayout(true);
    setShowPayModal(true);
    setShowPayoutModal(false);
    setCardIdentifier(cardId);
  };
  const onPayoutComplete = ({ amount }: { amount: string }) => {
    doPayout({ amount: parseFloat(amount), identifier: cardIdentifier })
      .unwrap()
      .then((result) => {
        setSuccessMessage(result);
      })
      .finally(() => {
        setShowPayModal(false);
        setShowPayModalLikePayout(false);
      });
  };
  const onResultChecked = () => {
    setSuccessMessage(undefined);
    setModalError(undefined);
    setShowPayModalLikePayout(false);
  };
  const onPhoneGrub = ({ phone }: { phone: string }) => {
    setPhone(phone);
    setShowPayModal(true);
    setShowPhoneGrub(false);
  };
  const onPayComplete = ({ amount }: { amount: string }) => {
    doPay({
      amount: parseFloat(amount),
      variant,
      identifier: phone,
      userId: null,
    })
      .unwrap()
      .then((e) => {
        if (e.confirmation.type == ConfirmationType.Redirect)
          window.open(e.confirmation.confirmationUrl);
        else if (e.confirmation.type == ConfirmationType.External) {
          setResultIcon("/img/phone.png");
          setSuccessMessage(e.text);
        }
      })
      .finally(() => setShowPayModal(false));
  };
  const onTopUp = () => {
    if (variant == "balance-phone") setShowPhoneGrub(true);
    else setShowPayModal(true);
  };
  return (
    <div
      className="px-5 pb-10 pt-6 bg-[#29377E]/10 xxl:max-w-[400px] xxl:w-full 
    max-w-[400px] w-[400px] rounded-l-[20px]"
    >
      {showPayoutModal && (
        <PayoutModal
          onClose={() => setShowPayoutModal(false)}
          onSubmit={onCardIdentifierGrub}
        ></PayoutModal>
      )}
      {showPhoneGrub && (
        <PayModal
          initialValues={{ phone: "" }}
          validationScheme={phoneValidationScheme}
          onClose={() => setShowPhoneGrub(false)}
          placeholderTexts={{ phone: "Ваш номер телефона" }}
          onSubmit={onPhoneGrub}
          buttonText="Далее"
        ></PayModal>
      )}
      {showPayModal && (
        <PayModal
          initialValues={{ amount: 50 }}
          validationScheme={validationSchema}
          isLoading={payoutLoading || payLoading}
          onClose={() => {
            setShowPayModal(false);
            setShowPayModalLikePayout(false);
          }}
          onSubmit={showPayModalLikePayout ? onPayoutComplete : onPayComplete}
          placeholderTexts={{
            amount: showPayModalLikePayout
              ? "Сумма к выводу"
              : "Сумма к оплате",
          }}
          buttonText={showPayModalLikePayout ? "Вывести" : "Пополнить"}
        ></PayModal>
      )}
      {(sucessMessage || modalError) && (
        <ResultModal
          onClose={onResultChecked}
          successMessage={sucessMessage}
          errorMessage={modalError}
          img={resultIcon}
        ></ResultModal>
      )}
      <h6 className="text-[20px] font-medium mb-9">Кошелек</h6>
      <div className="flex flex-col gap-[30px]">
        {PAYMENT_METHODS.map((method, i) => (
          <PaymentMethod
            key={i}
            {...method}
            isSelected={method.icon === variant}
            onClick={() => setVariant(method.icon)}
          ></PaymentMethod>
        ))}
      </div>
      <div className="flex items-center my-[30px] gap-10 justify-between xxl:flex-col xxl:gap-3">
        <button
          onClick={() => onTopUp()}
          className="font-semibold text-[14px] bg-saturateBlue hover:bg-secondaryBlue rounded-[10px] h-[60px] flex-middle w-full"
        >
          <Icon defaultColor="#fff" icon="load" />
          Пополнение
        </button>
        <button
          onClick={() => setShowPayoutModal(true)}
          className="font-semibold text-[14px] bg-saturateBlue hover:bg-secondaryBlue rounded-[10px] h-[60px] flex-middle w-full"
        >
          <Icon defaultColor="#fff" icon="moneyBack" />
          Вывод
        </button>
      </div>
      <img className="mx-auto" src="/img/payments/yoomoney.png" alt="ЮМАНИ" />
    </div>
  );
};
