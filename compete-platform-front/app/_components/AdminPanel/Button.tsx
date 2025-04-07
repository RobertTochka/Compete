type TButton = {
	children: React.ReactNode;
	onClick?: () => void;
};

export default function Button({ children, onClick }: TButton) {
	return (
		<button
			className="w-full text-center bg-saturateBlue hover:bg-secondaryBlue h-[42px] rounded-full"
			onClick={onClick}>
			{children}
		</button>
	);
}
