import React, { useEffect, useState } from "react";
import ModalWindow from "../ModalWindow";
import { IDefaultModalProps } from "../ChangeTeamNameModal/ChangeTeamNameModal";
import { Information } from "../Loading/Loading";

interface IPayoutModalProps extends IDefaultModalProps {}

export const PayoutModal = ({ onSubmit, ...rest }: IPayoutModalProps) => {
  const [init, setInit] = useState<boolean>(false);
  useEffect(() => {
    const payoutsData = new window.PayoutsData({
      type: "payout",
      account_id: "506341",
      success_callback: function (data: { payout_token: string }) {
        onSubmit(data.payout_token);
      },
      error_callback: function (error) {},
      customization: {
        colors: {
          control_primary: "#2563EB",
          background: "#1e2741",
          text: "white",
          border: "#1e2741",
        },
      },
    });

    payoutsData.render("payout-form").then(() => {});
    setInit(true);
    return () => {
      payoutsData.clearListeners();
    };
  });

  return (
    <ModalWindow {...rest}>
      {!init && <Information loading={!init} size={40}></Information>}
      <div id="payout-form"></div>
    </ModalWindow>
  );
};
